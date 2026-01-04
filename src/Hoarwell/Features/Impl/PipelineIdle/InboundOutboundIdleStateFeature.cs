using Hoarwell.Options.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hoarwell.Features;

/// <summary>
/// 入站出站空闲状态特征
/// </summary>
public class InboundOutboundIdleStateFeature : IInboundOutboundIdleStateFeature, IDisposable
{
    #region Private 字段

    private readonly ILogger _logger;

    private bool _isDisposed;

    #endregion Private 字段

    #region Protected 字段

    /// <summary>
    /// 当前检查所使用的 <see cref="CancellationTokenSource"/>
    /// </summary>
    protected readonly CancellationTokenSource TokenSource = new();

    /// <summary>
    /// 选项
    /// </summary>
    protected InboundOutboundIdleOptions Options;

    #endregion Protected 字段

    #region Public 事件

    /// <inheritdoc/>
    public event IdleStateTriggeredDelegate? OnIdleStateTriggered;

    #endregion Public 事件

    #region Public 属性

    /// <summary>
    /// 上下文
    /// </summary>
    public IHoarwellContext Context { get; }

    /// <summary>
    /// 空闲状态已触发
    /// </summary>
    public bool IdleStateTriggered { get; protected set; }

    /// <inheritdoc/>
    public DateTime? LastInbound { get; protected set; }

    /// <inheritdoc/>
    public DateTime? LastOutbound { get; protected set; }

    /// <summary>
    /// 已触发的空闲状态
    /// </summary>
    public IdleState? TriggeredIdleState { get; protected set; }

    #endregion Public 属性

    #region Public 构造函数

    /// <inheritdoc/>
    public InboundOutboundIdleStateFeature([ServiceKey] string applicationName,
                                           IHoarwellContextAccessor contextAccessor,
                                           IOptionsMonitor<InboundOutboundIdleOptions> optionsMonitor,
                                           ILogger<InboundOutboundIdleStateFeature> logger)
    {
        Context = contextAccessor?.Context ?? throw new ArgumentException($"can not get {nameof(IHoarwellContext)} now");
        Options = optionsMonitor.GetRequiredApplicationOptions(applicationName, static options => options);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _ = CycleChecking(TokenSource.Token);
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public virtual void UpdateInboundTime()
    {
        LastInbound = DateTime.UtcNow;
    }

    /// <inheritdoc/>
    public virtual void UpdateOutboundTime()
    {
        LastOutbound = DateTime.UtcNow;
    }

    #endregion Public 方法

    #region Protected 方法

    /// <summary>
    /// 循环检查空闲
    /// </summary>
    /// <param name="cancellationToken"></param>
    protected virtual async Task CycleChecking(CancellationToken cancellationToken)
    {
        await Task.Yield();

        IdleState idleState = default;
        TimeSpan delay = TimeSpan.Zero;

        var inboundIdleTimeout = Options.InboundIdleTimeout;
        var outboundIdleTimeout = Options.OutboundIdleTimeout;

        while (!cancellationToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            TimeSpan? nextInboundCheckDelay = inboundIdleTimeout.HasValue
                                              ? LastInbound.HasValue
                                                ? inboundIdleTimeout.Value - (now - LastInbound.Value)  //设置了入站超时且有最后入站时间
                                                : inboundIdleTimeout.Value  //设置了入站超时但没有最后入站时间
                                              : null;   //未设置入站超时

            TimeSpan? nextOutboundCheckDelay = outboundIdleTimeout.HasValue
                                               ? LastOutbound.HasValue
                                                 ? outboundIdleTimeout.Value - (now - LastOutbound.Value)   //设置了出站超时且有最后出站时间
                                                 : outboundIdleTimeout.Value    //设置了出站超时但没有最后出站时间
                                               : null;  //未设置出站超时

            if (nextInboundCheckDelay < nextOutboundCheckDelay)
            {
                idleState = IdleState.InboundIdle;
                delay = nextInboundCheckDelay.Value;
            }
            else if (nextOutboundCheckDelay < nextInboundCheckDelay)
            {
                idleState = IdleState.OutboundIdle;
                delay = nextOutboundCheckDelay.Value;
            }
            else if (nextInboundCheckDelay.HasValue)
            {
                idleState = IdleState.InboundIdle;
                delay = nextInboundCheckDelay.Value;
            }
            else if (nextOutboundCheckDelay.HasValue)
            {
                idleState = IdleState.OutboundIdle;
                delay = nextOutboundCheckDelay.Value;
            }

            await DelayAsync(delay, cancellationToken).ConfigureAwait(false);

            switch (idleState)
            {
                case IdleState.InboundIdle:
                    if (LastInbound is null
                        || DateTime.UtcNow - LastInbound.Value > inboundIdleTimeout!.Value)
                    {
                        if (OnIdleStateTriggered is { } onIdleStateTriggered
                            && await onIdleStateTriggered(this, Context, idleState).ConfigureAwait(false))
                        {
                            UpdateInboundTime();
                            continue;
                        }
                        await TriggerIdleStateAsync(idleState).ConfigureAwait(false);
                    }
                    break;

                case IdleState.OutboundIdle:
                    if (LastOutbound is null
                        || DateTime.UtcNow - LastOutbound.Value > outboundIdleTimeout!.Value)
                    {
                        if (OnIdleStateTriggered is { } onIdleStateTriggered
                            && await onIdleStateTriggered(this, Context, idleState).ConfigureAwait(false))
                        {
                            UpdateOutboundTime();
                            continue;
                        }
                        await TriggerIdleStateAsync(idleState).ConfigureAwait(false);
                    }
                    break;

                case IdleState.Unknown:
                default:
                    {
                        _logger.LogCritical("invalid idle state {IdleState} for checking. forced delay for next check.", idleState);
                        await DelayAsync(TimeSpan.FromMinutes(1), cancellationToken).ConfigureAwait(false);
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 延时
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken) => Task.Delay(delay, cancellationToken);

    /// <summary>
    /// 空闲状态触发
    /// </summary>
    /// <param name="idleState"></param>
    /// <returns></returns>
    protected virtual ValueTask TriggerIdleStateAsync(IdleState idleState)
    {
        IdleStateTriggered = true;
        TriggeredIdleState = idleState;
        Context.Abort(idleState);
        return default;
    }

    #endregion Protected 方法

    #region Dispose

    /// <summary>
    ///
    /// </summary>
    ~InboundOutboundIdleStateFeature()
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
        if (!_isDisposed)
        {
            _isDisposed = true;

            try
            {
                TokenSource.Cancel();
            }
            catch { }

            TokenSource.Dispose();
        }
    }

    #endregion Dispose
}
