using Hoarwell.ExecutionPipeline;
using Hoarwell.ExecutionPipeline.Internal;

namespace Hoarwell.ExecutionPipeline.Build;

/// <summary>
/// 执行管道构建器
/// </summary>
public static class ExecutionPipelineBuilder
{
    #region Public 方法

    /// <summary>
    /// 创建一个上下文为 <typeparamref name="TContext"/> ，初始输入为 <typeparamref name="TInput"/> 的执行管道构建器
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <returns></returns>
    public static IExecutionPipelineBuilder<TContext, TInput> Create<TContext, TInput>()
        where TContext : IExecutionPipelineContext
    {
        return new ExecutionPipelineBuilder<TContext, TInput>(new());
    }

    #endregion Public 方法
}

internal sealed class ExecutionPipelineBuilder<TContext, TInput>(ExecutionPipelineBuilderContext context)
    : ExecutionPipelineBuilderChainNode<TContext, TInput>(context, PipelineRunHelper.BuildStartPipelineInvokeDelegate<TContext, TInput>)
    , IExecutionPipelineBuilder<TContext, TInput>
    where TContext : IExecutionPipelineContext
{
    #region Public 方法

    public PipelineInvokeDelegate<TContext, TInput> Build()
    {
        var pipelineBuildDelegate = Context.PipelineBuildDelegate
                                    ?? throw new InvalidOperationException("no middleware configured");
        return (PipelineInvokeDelegate<TContext, TInput>)pipelineBuildDelegate(null);
    }

    #endregion Public 方法
}
