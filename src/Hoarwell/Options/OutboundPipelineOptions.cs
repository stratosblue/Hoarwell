using Hoarwell.ExecutionPipeline;

namespace Hoarwell.Options;

/// <summary>
/// 出站管道选项
/// </summary>
/// <typeparam name="TContext"></typeparam>
public class OutboundPipelineOptions<TContext>
    where TContext : IHoarwellContext
{
    #region Public 属性

    /// <summary>
    /// 出站管道执行委托
    /// </summary>
    public PipelineInvokeDelegate<TContext, OutboundMetadata>? OutboundPipelineInvokeDelegate { get; set; }

    #endregion Public 属性
}
