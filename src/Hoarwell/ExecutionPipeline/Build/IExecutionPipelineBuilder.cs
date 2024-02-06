namespace Hoarwell.ExecutionPipeline.Build;

/// <summary>
/// 上下文为 <typeparamref name="TContext"/> ，初始输入为 <typeparamref name="TInput"/> 的执行管道构建器
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TInput"></typeparam>
public interface IExecutionPipelineBuilder<TContext, TInput>
    : IExecutionPipelineBuilderChainNode<TContext, TInput>
    where TContext : IExecutionPipelineContext
{
    #region Public 方法

    /// <summary>
    /// 构建管道执行委托
    /// </summary>
    /// <returns></returns>
    PipelineInvokeDelegate<TContext, TInput> Build();

    #endregion Public 方法
}
