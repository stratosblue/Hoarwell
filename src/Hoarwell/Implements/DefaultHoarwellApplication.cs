using Hoarwell.ExecutionPipeline;
using Hoarwell.Features;
using Hoarwell.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hoarwell;

/// <summary>
/// 默认的 Hoarwell 应用程序
/// </summary>
/// <typeparam name="TInputter"></typeparam>
/// <typeparam name="TOutputter"></typeparam>
public class DefaultHoarwellApplication<TInputter, TOutputter> : DefaultHoarwellApplication<HoarwellContext, TInputter, TOutputter>
{
    #region Public 构造函数

    /// <inheritdoc cref="DefaultHoarwellApplication{TInputter, TOutputter}"/>
    public DefaultHoarwellApplication([ServiceKey] string applicationName,
                                      IOptionsMonitor<InboundPipelineOptions<HoarwellContext, TInputter>> inboundPipelineOptionsMonitor,
                                      IOptionsMonitor<OutboundPipelineOptions<HoarwellContext>> outboundPipelineOptionsMonitor,
                                      IServiceProvider serviceProvider)
        : base(applicationName, inboundPipelineOptionsMonitor, outboundPipelineOptionsMonitor, serviceProvider)
    {
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public override ValueTask<HoarwellContext> CreateContext(IFeatureCollection features)
    {
        var duplexPipeFeature = features.RequiredFeature<IDuplexPipeFeature<TInputter, TOutputter>>();

        var adaptedOutputter = OutputterAdapter.Adapt(duplexPipeFeature.Outputter, SerializeOutboundMessageDelegate);

        var context = new HoarwellContext(ApplicationName, features, adaptedOutputter);

        return new(context);
    }

    #endregion Public 方法
}

/// <summary>
/// 默认的 Hoarwell 应用程序
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TInputter"></typeparam>
/// <typeparam name="TOutputter"></typeparam>
public abstract class DefaultHoarwellApplication<TContext, TInputter, TOutputter>
    : IHoarwellApplication<TContext, TInputter, TOutputter>
    where TContext : IHoarwellContext
{
    #region Protected 字段

    /// <summary>
    /// 输入管道执行委托
    /// </summary>
    protected readonly PipelineInvokeDelegate<TContext, TInputter> InboundPipelineInvokeDelegate;

    /// <summary>
    /// 输出管道执行委托
    /// </summary>
    protected readonly PipelineInvokeDelegate<TContext, OutboundMetadata> OutboundPipelineInvokeDelegate;

    /// <summary>
    /// 输出器适配器
    /// </summary>
    protected readonly IOutputterAdapter<TOutputter> OutputterAdapter;

    /// <summary>
    /// 消息输出序列化委托
    /// </summary>
    protected readonly SerializeOutboundMessageDelegate SerializeOutboundMessageDelegate;

    #endregion Protected 字段

    #region Public 属性

    /// <inheritdoc/>
    public string ApplicationName { get; }

    #endregion Public 属性

    #region Public 构造函数

    /// <inheritdoc cref="DefaultHoarwellApplication{TContext, TInputter, TOutputter}"/>
    public DefaultHoarwellApplication([ServiceKey] string applicationName,
                                      IOptionsMonitor<InboundPipelineOptions<TContext, TInputter>> inboundPipelineOptionsMonitor,
                                      IOptionsMonitor<OutboundPipelineOptions<TContext>> outboundPipelineOptionsMonitor,
                                      IServiceProvider serviceProvider)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(applicationName);

        ApplicationName = applicationName;

        InboundPipelineInvokeDelegate = inboundPipelineOptionsMonitor.GetRequiredApplicationOptions(applicationName,
                                                                                                     options => options.InboundPipelineInvokeDelegate,
                                                                                                     $"Inbound pipeline not configured correctly for application \"{applicationName}\"");

        OutboundPipelineInvokeDelegate = outboundPipelineOptionsMonitor.GetRequiredApplicationOptions(applicationName,
                                                                                                       options => options.OutboundPipelineInvokeDelegate,
                                                                                                       $"Outbound pipeline not configured correctly for application \"{applicationName}\"");

        OutputterAdapter = serviceProvider.GetRequiredKeyedService<IOutputterAdapter<TOutputter>>(applicationName);

        SerializeOutboundMessageDelegate = SerializeOutboundMessageAsync;
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public abstract ValueTask<TContext> CreateContext(IFeatureCollection features);

    /// <inheritdoc/>
    public virtual ValueTask DisposeContext(TContext context)
    {
        context.Dispose();
        return default;
    }

    /// <inheritdoc/>
    public virtual Task ExecuteAsync(TContext context, TInputter inputter, TOutputter outputter)
    {
        var inboundRunningTask = InboundPipelineInvokeDelegate(context, inputter);
        return inboundRunningTask;
    }

    #endregion Public 方法

    #region Protected 方法

    /// <summary>
    /// 使用上下文 <paramref name="context"/> 执行 <paramref name="output"/> 的序列化操作
    /// </summary>
    /// <param name="context"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    protected virtual Task SerializeOutboundMessageAsync(IHoarwellContext context, OutboundMetadata output)
    {
        return OutboundPipelineInvokeDelegate((TContext)context, output);
    }

    #endregion Protected 方法
}
