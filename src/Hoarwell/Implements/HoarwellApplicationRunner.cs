using System.Net.Sockets;
using System.Threading.Channels;
using Hoarwell.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoarwell;

/// <summary>
/// <inheritdoc cref="IHoarwellApplicationRunner"/>
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TApplication"></typeparam>
/// <typeparam name="TInputter"></typeparam>
/// <typeparam name="TOutputter"></typeparam>
public abstract class HoarwellApplicationRunner<TContext, TApplication, TInputter, TOutputter>
    : IHoarwellApplicationRunner
    where TContext : IHoarwellContext
    where TApplication : IHoarwellApplication<TContext, TInputter, TOutputter>
{
    #region Public 事件

    /// <inheritdoc/>
    public event HoarwellContextActiveCallback? OnContextActive;

    /// <inheritdoc/>
    public event HoarwellContextInactiveCallback? OnContextInactive;

    #endregion Public 事件

    #region Private 字段

    private readonly CancellationTokenSource _runningCTS;

    /// <summary>
    /// 应用程序状态
    /// <see cref="ApplicationStatus"/>
    /// </summary>
    private volatile int _applicationStatus = (int)ApplicationStatus.Initialized;

    private IEnumerable<IDuplexPipeConnector<TInputter, TOutputter>>? _connectors;

    #endregion Private 字段

    #region Public 属性

    /// <inheritdoc/>
    public CancellationToken ApplicationStopping { get; }

    /// <summary>
    /// 是否已处置
    /// </summary>
    public bool Disposed => _applicationStatus == (int)ApplicationStatus.Disposed;

    #endregion Public 属性

    #region Protected 属性

    /// <summary>
    /// 使用的应用程序
    /// </summary>
    protected IHoarwellApplication<TContext, TInputter, TOutputter> Application { get; }

    /// <summary>
    /// 连接器列表
    /// </summary>
    protected IEnumerable<IDuplexPipeConnector<TInputter, TOutputter>>? Connectors => _connectors;

    /// <summary>
    /// 上下文事件管道
    /// </summary>
    protected Channel<ContextEventRecord> ContextEventChannel { get; }

    /// <summary>
    /// 双工管道工厂
    /// </summary>
    protected IDuplexPipeConnectorFactory<TInputter, TOutputter> DuplexPipeConnectorFactory { get; }

    /// <inheritdoc cref="ILogger"/>
    protected ILogger Logger { get; }

    /// <inheritdoc cref="IServiceScopeFactory"/>
    protected IServiceScopeFactory ServiceScopeFactory { get; }

    #endregion Protected 属性

    #region Public 构造函数

    /// <inheritdoc cref="HoarwellApplicationRunner{TContext, TApplication, TInputter, TOutputter}"/>
    public HoarwellApplicationRunner([ServiceKey] string applicationName,
                                     IServiceProvider serviceProvider,
                                     IServiceScopeFactory serviceScopeFactory,
                                     ILogger logger)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(applicationName);
        ArgumentNullExceptionHelper.ThrowIfNull(serviceProvider);
        ArgumentNullExceptionHelper.ThrowIfNull(serviceScopeFactory);
        ArgumentNullExceptionHelper.ThrowIfNull(logger);

        DuplexPipeConnectorFactory = serviceProvider.GetRequiredKeyedService<IDuplexPipeConnectorFactory<TInputter, TOutputter>>(applicationName);
        Application = serviceProvider.GetRequiredKeyedService<IHoarwellApplication<TContext, TInputter, TOutputter>>(applicationName);

        ServiceScopeFactory = serviceScopeFactory;
        Logger = logger;

        _runningCTS = new();
        ApplicationStopping = _runningCTS.Token;

        ContextEventChannel = Channel.CreateUnbounded<ContextEventRecord>(new() { SingleReader = true });

        Task.Factory.StartNew(function: async state =>
        {
            var runner = (HoarwellApplicationRunner<TContext, TApplication, TInputter, TOutputter>)state!;

            var cancellationToken = runner.ApplicationStopping;

            await foreach (var (context, isActive) in runner.ContextEventChannel.Reader.ReadAllAsync(cancellationToken))
            {
                try
                {
                    if (isActive)
                    {
                        runner.OnContextActive?.Invoke(context);
                    }
                    else
                    {
                        runner.OnContextInactive?.Invoke(context);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "An error occurred while fire context {Context} event", context);
                }
            }
        }, state: this, cancellationToken: ApplicationStopping, creationOptions: TaskCreationOptions.None, scheduler: TaskScheduler.Default);
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public virtual async ValueTask DisposeAsync()
    {
        if (_applicationStatus == (int)ApplicationStatus.Disposed)
        {
            return;
        }

        _runningCTS.SilenceRelease();

        ContextEventChannel.Writer.TryComplete();

        if (Interlocked.Exchange(ref _connectors, null) is { } connectors)
        {
            await StopConnectorsAsync(connectors, default).ConfigureAwait(false);
        }

        _applicationStatus = (int)ApplicationStatus.Disposed;

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public virtual async Task StartAsync(CancellationToken startCancellationToken)
    {
        ThrowIfDisposed();

        if (Interlocked.CompareExchange(ref _applicationStatus, (int)ApplicationStatus.Starting, (int)ApplicationStatus.Initialized) is { } status
            && status != (int)ApplicationStatus.Initialized)
        {
            throw new InvalidOperationException($"The application runner can not start from the status \"{(ApplicationStatus)status}\"");
        }

        using var localCts = CancellationTokenSource.CreateLinkedTokenSource(startCancellationToken, ApplicationStopping);

        startCancellationToken = localCts.Token;

        List<IDuplexPipeConnector<TInputter, TOutputter>> connectors = [];

        try
        {
            await foreach (var connector in DuplexPipeConnectorFactory.GetAsync(startCancellationToken))
            {
                connectors.Add(connector);
            }

            if (connectors.Count == 0)
            {
                throw new InvalidOperationException("No connector got from factory");
            }

            if (Interlocked.Exchange(ref _connectors, connectors) is { } oldConnectors)
            {
                //理论上不会到这里
                await StopConnectorsAsync(oldConnectors, startCancellationToken).ConfigureAwait(false);
            }

            var connectTasks = connectors.Select(RunConnectLoopAsync).ToList();

            Interlocked.CompareExchange(ref _applicationStatus, (int)ApplicationStatus.Running, (int)ApplicationStatus.Starting);
        }
        catch
        {
            Interlocked.CompareExchange(ref _applicationStatus, (int)ApplicationStatus.Stopped, (int)ApplicationStatus.Starting);
            _ = StopConnectorsAsync(connectors, startCancellationToken);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        if (Interlocked.CompareExchange(ref _applicationStatus, (int)ApplicationStatus.Stopping, (int)ApplicationStatus.Running) is { } status
            && status != (int)ApplicationStatus.Running)
        {
            throw new InvalidOperationException($"The application runner can not stop from the status \"{(ApplicationStatus)status}\"");
        }

        try
        {
            await DoStopAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            Interlocked.CompareExchange(ref _applicationStatus, (int)ApplicationStatus.Stopped, (int)ApplicationStatus.Stopping);
        }
    }

    #endregion Public 方法

    #region Protected 方法

    /// <summary>
    /// 执行停止
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual Task DoStopAsync(CancellationToken cancellationToken)
    {
        ContextEventChannel.Writer.TryComplete();

        return StopConnectorsAsync(_connectors, cancellationToken);
    }

    /// <summary>
    /// 触发上下文激活委托
    /// </summary>
    /// <param name="context"></param>
    protected virtual void FireContextActive(TContext context)
    {
        if (OnContextActive is not null)
        {
            //字面上来说不会写入失败
            ContextEventChannel.Writer.TryWrite(new(context, true));
        }
    }

    /// <summary>
    /// 触发上下文失活委托
    /// </summary>
    /// <param name="context"></param>
    protected virtual void FireContextInactive(TContext context)
    {
        if (OnContextInactive is not null)
        {
            //字面上来说不会写入失败
            ContextEventChannel.Writer.TryWrite(new(context, false));
        }
    }

    /// <summary>
    /// 运行应用程序
    /// </summary>
    /// <param name="application"></param>
    /// <param name="duplexPipeContext"></param>
    /// <returns></returns>
    protected virtual async Task RunApplicationAsync(IHoarwellApplication<TContext, TInputter, TOutputter> application, IDuplexPipeContext<TInputter, TOutputter> duplexPipeContext)
    {
        TContext? context = default;
        try
        {
            var features = new FeatureCollection(duplexPipeContext.Features);

            await using var serviceScope = ServiceScopeFactory.CreateAsyncScope();

            features.Set<IServiceProviderFeature>(new PipeServiceProviderFeature(serviceScope.ServiceProvider));
            features.Set<IDuplexPipeFeature<TInputter, TOutputter>>(duplexPipeContext);

            context = await application.CreateContext(features).ConfigureAwait(false);

            if (serviceScope.ServiceProvider.GetService<HoarwellContextAccessor>() is { } contextAccessor)
            {
                contextAccessor.Context = context;
            }

            FireContextActive(context);

            await application.ExecuteAsync(context, duplexPipeContext.Inputter, duplexPipeContext.Outputter).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (ex is OperationCanceledException or IOException or SocketException
                && context?.ExecutionAborted.IsCancellationRequested == true)
            {
                Logger.LogWarning(ex, "An error occurred while running application");
                return;
            }
            Logger.LogError(ex, "An error occurred while running application");
            duplexPipeContext.Abort();
        }
        finally
        {
            if (context is not null)
            {
                FireContextInactive(context);
                await application.DisposeContext(context).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// 运行连接循环
    /// </summary>
    /// <param name="connector"></param>
    /// <returns></returns>
    protected virtual async Task RunConnectLoopAsync(IDuplexPipeConnector<TInputter, TOutputter> connector)
    {
        var cancellationToken = ApplicationStopping;
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var duplexPipeContext = await connector.ConnectAsync(cancellationToken).ConfigureAwait(false);

                _ = RunApplicationAsync(Application, duplexPipeContext);
            }
        }
        catch (Exception ex)
        {
            if (ex is OperationCanceledException
                && cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            Logger.LogCritical(ex, "An error occurred while duplex pipe connector {Connector} running", connector);

            try
            {
                await connector.StopAsync(cancellationToken).ConfigureAwait(false);
                await connector.DisposeAsync().ConfigureAwait(false);
            }
            catch (Exception disposeEx)
            {
                Logger.LogError(disposeEx, "An error occurred while dispose the error duplex pipe connector {Connector}", connector);
            }
        }
    }

    /// <summary>
    /// 停止连接器列表
    /// </summary>
    /// <param name="connectors"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task StopConnectorsAsync(IEnumerable<IDuplexPipeConnector<TInputter, TOutputter>>? connectors, CancellationToken cancellationToken)
    {
        if (connectors is null)
        {
            return;
        }
        foreach (var connector in connectors)
        {
            try
            {
                await connector.StopAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while stopping connector {Connector}", connector);
            }
        }
    }

    /// <summary>
    /// 如果已处置则抛出异常
    /// </summary>
    protected void ThrowIfDisposed()
    {
        ObjectDisposedExceptionHelper.ThrowIf(_applicationStatus == (int)ApplicationStatus.Disposed, this);
    }

    #endregion Protected 方法

    #region Protected 类型

    /// <summary>
    /// 应用程序状态
    /// </summary>
    protected enum ApplicationStatus : int
    {
        /// <summary>
        /// 初始化
        /// </summary>
        Initialized,

        /// <summary>
        /// 启动中
        /// </summary>
        Starting,

        /// <summary>
        /// 运行中
        /// </summary>
        Running,

        /// <summary>
        /// 停止中
        /// </summary>
        Stopping,

        /// <summary>
        /// 已停止
        /// </summary>
        Stopped,

        /// <summary>
        /// 已处置
        /// </summary>
        Disposed,
    }

    /// <summary>
    /// 上下文事件信息
    /// </summary>
    /// <param name="Context"></param>
    /// <param name="IsActive">是否为激活</param>
    protected record struct ContextEventRecord(IHoarwellContext Context, bool IsActive);

    #endregion Protected 类型
}
