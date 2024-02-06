using Hoarwell.ExecutionPipeline;

namespace Hoarwell.Options;

/// <summary>
/// 入站管道选项
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TInputter"></typeparam>
public class InboundPipelineOptions<TContext, TInputter>
    where TContext : IHoarwellContext
{
    #region Public 属性

    /// <summary>
    /// 入站管道执行委托
    /// </summary>
    public PipelineInvokeDelegate<TContext, TInputter>? InboundPipelineInvokeDelegate { get; set; }

    #endregion Public 属性
}
