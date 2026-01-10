using Hoarwell.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Hoarwell;

/// <summary>
/// 默认的 <inheritdoc cref="IHoarwellContext"/>
/// </summary>
public class HoarwellContext : IHoarwellContext
{
    #region Private 字段

    private readonly CancellationTokenSource _abortingCTS = new();

    private readonly IPipeLifetimeFeature _pipeLifetimeFeature;

    private volatile bool _isAborting = false;

    private volatile bool _isDisposed = false;

    private ILogger? _logger;

    #endregion Private 字段

    #region Private 属性

    private ILogger Logger => _logger ??= Services?.GetService<ILoggerFactory>()?.CreateLogger("Hoarwell.HoarwellContext") ?? NullLogger.Instance;

    #endregion Private 属性

    #region Public 属性

    /// <inheritdoc/>
    public string ApplicationName { get; }

    /// <inheritdoc/>
    public object? CloseReason { get; private set; }

    /// <inheritdoc/>
    public CancellationToken ExecutionAborted => _pipeLifetimeFeature.PipeClosed;

    /// <inheritdoc/>
    public CancellationToken ExecutionAborting => _abortingCTS.Token;

    /// <inheritdoc/>
    public IFeatureCollection Features { get; }

    /// <inheritdoc/>
    public IOutputter Outputter { get; }

    /// <inheritdoc/>
    public IDictionary<object, object> Properties { get; }

    /// <inheritdoc/>
    public IServiceProvider Services { get; }

    #endregion Public 属性

    #region Protected Internal 构造函数

    /// <inheritdoc cref="HoarwellContext"/>
    protected internal HoarwellContext(string applicationName,
                                       IFeatureCollection features,
                                       IOutputter outputter)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(applicationName);
        ArgumentNullExceptionHelper.ThrowIfNull(features);
        ArgumentNullExceptionHelper.ThrowIfNull(outputter);

        ApplicationName = applicationName;
        Features = features;
        Outputter = outputter;

        Services = features.RequiredFeature<IServiceProviderFeature>().Services;
        _pipeLifetimeFeature = Features.RequiredFeature<IPipeLifetimeFeature>();

        //HACK 可能出现并发问题
        Properties = new Dictionary<object, object>();
    }

    #endregion Protected Internal 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public void Abort(object? reason)
    {
        if (_isAborting)
        {
            return;
        }

        _isAborting = true;

        try
        {
            CloseReason = reason;

            if (Logger.IsEnabled(LogLevel.Information))
            {
                Logger.LogInformation("Context is aborting: {Reason}", reason);
            }

            try
            {
                if (!ExecutionAborted.IsCancellationRequested)
                {
                    try
                    {
                        _abortingCTS.Cancel();
                    }
                    catch (Exception ex)
                    {
                        if (Logger.IsEnabled(LogLevel.Warning))
                        {
                            Logger.LogWarning(ex, "An exception occurred while notify closing");
                        }
                    }

                    _pipeLifetimeFeature.Abort();
                }

                Dispose(false);
            }
            catch (Exception ex)
            {
                if (Logger.IsEnabled(LogLevel.Warning))
                {
                    Logger.LogWarning(ex, "An exception occurred while the context pipe was closing");
                }
            }
        }
        finally
        {
            _isAborting = false;
        }
    }

    #endregion Public 方法

    #region IDisposable

    /// <summary>
    ///
    /// </summary>
    ~HoarwellContext()
    {
        Dispose(disposing: false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed
            || _isAborting)
        {
            return;
        }

        if (disposing)
        {
            Abort("Context disposing");
        }

        Outputter.Dispose();
        _abortingCTS.Dispose();

        _isDisposed = true;
    }

    #endregion IDisposable
}
