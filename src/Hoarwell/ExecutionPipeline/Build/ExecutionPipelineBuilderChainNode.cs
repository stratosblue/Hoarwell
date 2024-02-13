using Hoarwell.ExecutionPipeline.Internal;

namespace Hoarwell.ExecutionPipeline.Build;

internal class ExecutionPipelineBuilderChainNode<TContext, TInput> : IExecutionPipelineBuilderChainNode<TContext, TInput>
    where TContext : IExecutionPipelineContext
{
    #region Private 字段

    private readonly Func<object?, object> _previousPipelineBuildDelegate;

    #endregion Private 字段

    #region Public 属性

    public ExecutionPipelineBuilderContext Context { get; }

    public int Version { get; }

    #endregion Public 属性

    #region Public 构造函数

    public ExecutionPipelineBuilderChainNode(ExecutionPipelineBuilderContext context, Func<object?, object> previousPipelineBuildDelegate)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        _previousPipelineBuildDelegate = previousPipelineBuildDelegate ?? throw new ArgumentNullException(nameof(previousPipelineBuildDelegate));

        Version = context.Version;
    }

    #endregion Public 构造函数

    #region Public 方法

    public IExecutionPipelineBuilderChainNode<TContext, TOutput> Use<TMiddleware, TOutput>(object? serviceKey = null) where TMiddleware : IPipelineMiddleware<TContext, TInput, TOutput>
    {
        Func<object?, object> pipelineBuildDelegate = next => _previousPipelineBuildDelegate(PipelineRunHelper.BuildInvokeDelegate<TMiddleware, TContext, TInput, TOutput>(next, serviceKey));
        Context.Update(pipelineBuildDelegate, Version);
        return new ExecutionPipelineBuilderChainNode<TContext, TOutput>(Context, pipelineBuildDelegate);
    }

    public IExecutionPipelineBuilderChainNode<TContext, TOutput> Use<TMiddleware, TOutput>(TMiddleware middleware) where TMiddleware : IPipelineMiddleware<TContext, TInput, TOutput>
    {
        Func<object?, object> pipelineBuildDelegate = next => _previousPipelineBuildDelegate(PipelineRunHelper.BuildInvokeDelegate<TMiddleware, TContext, TInput, TOutput>(middleware, next));
        Context.Update(pipelineBuildDelegate, Version);
        return new ExecutionPipelineBuilderChainNode<TContext, TOutput>(Context, pipelineBuildDelegate);
    }

    #endregion Public 方法
}
