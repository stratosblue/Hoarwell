namespace Hoarwell.ExecutionPipeline.Build;

/// <summary>
/// 执行管道构建器的链式构建节点
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TInput">下一个节点的输入类型</typeparam>
public interface IExecutionPipelineBuilderChainNode<TContext, TInput>
    where TContext : IExecutionPipelineContext
{
    #region Public 方法

    /// <summary>
    /// 添加中间件 <typeparamref name="TMiddleware"/> 该中间件接受的输入类型为 <typeparamref name="TInput"/> , 输出类型为 <typeparamref name="TOutput"/>
    /// </summary>
    /// <typeparam name="TMiddleware"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="serviceKey"></param>
    /// <returns></returns>
    IExecutionPipelineBuilderChainNode<TContext, TOutput> Use<TMiddleware, TOutput>(object? serviceKey = null) where TMiddleware : IPipelineMiddleware<TContext, TInput, TOutput>;

    /// <summary>
    /// 添加中间件 <typeparamref name="TMiddleware"/> 该中间件接受的输入类型为 <typeparamref name="TInput"/> , 输出类型为 <typeparamref name="TOutput"/>
    /// </summary>
    /// <typeparam name="TMiddleware"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="middleware"></param>
    /// <returns></returns>
    IExecutionPipelineBuilderChainNode<TContext, TOutput> Use<TMiddleware, TOutput>(TMiddleware middleware) where TMiddleware : IPipelineMiddleware<TContext, TInput, TOutput>;

    #endregion Public 方法
}
