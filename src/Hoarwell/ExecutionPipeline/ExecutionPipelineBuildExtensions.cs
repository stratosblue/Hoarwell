using System.ComponentModel;
using Hoarwell.ExecutionPipeline.Build;

namespace Hoarwell.ExecutionPipeline;

/// <summary>
/// 执行管道构建拓展方法
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ExecutionPipelineBuildExtensions
{
    #region Run

    /// <summary>
    /// 添加终结点, 该终结点接受的输入类型为 <typeparamref name="TInput"/>
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <param name="chainNode"></param>
    /// <param name="middleware"></param>
    /// <returns></returns>
    public static void Run<TContext, TInput>(this IExecutionPipelineBuilderChainNode<TContext, TInput> chainNode,
                                             Func<TInput, Task> middleware)
        where TContext : IExecutionPipelineContext
    {
        chainNode.Use<IPipelineMiddleware<TContext, TInput, object?>, object?>(DelegatingPipelineMiddleware.CreateEndpoint<TContext, TInput>(middleware));
    }

    /// <summary>
    /// 添加终结点, 该终结点接受的输入类型为 <typeparamref name="TInput"/>
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <param name="chainNode"></param>
    /// <param name="middleware"></param>
    /// <returns></returns>
    public static void Run<TContext, TInput>(this IExecutionPipelineBuilderChainNode<TContext, TInput> chainNode,
                                             Func<TContext, TInput, Task> middleware)
        where TContext : IExecutionPipelineContext
    {
        chainNode.Use<IPipelineMiddleware<TContext, TInput, object?>, object?>(DelegatingPipelineMiddleware.CreateEndpoint(middleware));
    }

    #endregion Run

    #region Use

    /// <summary>
    /// 添加中间件, 该中间件接受的输入类型为 <typeparamref name="TInput"/> , 输出类型为 <typeparamref name="TOutput"/>
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="chainNode"></param>
    /// <param name="middleware"></param>
    /// <returns></returns>
    public static IExecutionPipelineBuilderChainNode<TContext, TOutput> Use<TContext, TInput, TOutput>(this IExecutionPipelineBuilderChainNode<TContext, TInput> chainNode,
                                                                                                       Func<TInput, Task<TOutput>> middleware)
        where TContext : IExecutionPipelineContext
    {
        return chainNode.Use<IPipelineMiddleware<TContext, TInput, TOutput>, TOutput>(DelegatingPipelineMiddleware.CreateMiddleware<TContext, TInput, TOutput>(middleware));
    }

    /// <summary>
    /// 添加中间件, 该中间件接受的输入类型为 <typeparamref name="TInput"/> , 输出类型为 <typeparamref name="TOutput"/>
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="chainNode"></param>
    /// <param name="middleware"></param>
    /// <returns></returns>
    public static IExecutionPipelineBuilderChainNode<TContext, TOutput> Use<TContext, TInput, TOutput>(this IExecutionPipelineBuilderChainNode<TContext, TInput> chainNode,
                                                                                                       Func<TContext, TInput, Task<TOutput>> middleware)
        where TContext : IExecutionPipelineContext
    {
        return chainNode.Use<IPipelineMiddleware<TContext, TInput, TOutput>, TOutput>(DelegatingPipelineMiddleware.CreateMiddleware(middleware));
    }

    /// <summary>
    /// 添加中间件, 该中间件接受的输入类型为 <typeparamref name="TInput"/> , 输出类型为 <typeparamref name="TOutput"/>
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="chainNode"></param>
    /// <param name="middleware"></param>
    /// <returns></returns>
    public static IExecutionPipelineBuilderChainNode<TContext, TOutput> Use<TContext, TInput, TOutput>(this IExecutionPipelineBuilderChainNode<TContext, TInput> chainNode,
                                                                                                       Func<TContext, TInput, PipelineInvokeDelegate<TContext, TOutput>, Task> middleware)
        where TContext : IExecutionPipelineContext
    {
        return chainNode.Use<IPipelineMiddleware<TContext, TInput, TOutput>, TOutput>(DelegatingPipelineMiddleware.CreateMiddleware(middleware));
    }

    #endregion Use
}
