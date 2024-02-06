using System.Diagnostics;

namespace Hoarwell.ExecutionPipeline;

/// <summary>
/// 基于委托的中间件
/// </summary>
public static class DelegatingPipelineMiddleware
{
    #region Endpoint

    /// <summary>
    /// 创建一个基于委托 <paramref name="middleware"/> 的终结点
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <param name="middleware"></param>
    /// <returns></returns>
    public static IPipelineMiddleware<TContext, TInput, object?> CreateEndpoint<TContext, TInput>(Func<TInput, Task> middleware)
        where TContext : IExecutionPipelineContext
    {
        return new DelegatingPipelineEndpoint1<TContext, TInput>(middleware);
    }

    /// <summary>
    /// 创建一个基于委托 <paramref name="middleware"/> 的终结点
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <param name="middleware"></param>
    /// <returns></returns>
    public static IPipelineMiddleware<TContext, TInput, object?> CreateEndpoint<TContext, TInput>(Func<TContext, TInput, Task> middleware)
        where TContext : IExecutionPipelineContext
    {
        return new DelegatingPipelineEndpoint2<TContext, TInput>(middleware);
    }

    #endregion Endpoint

    #region Middleware

    /// <summary>
    /// 创建一个基于委托 <paramref name="middleware"/> 的中间件
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="middleware"></param>
    /// <returns></returns>
    public static IPipelineMiddleware<TContext, TInput, TOutput> CreateMiddleware<TContext, TInput, TOutput>(Func<TInput, Task<TOutput>> middleware)
        where TContext : IExecutionPipelineContext
    {
        return new DelegatingPipelineMiddleware1<TContext, TInput, TOutput>(middleware);
    }

    /// <summary>
    /// 创建一个基于委托 <paramref name="middleware"/> 的中间件
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="middleware"></param>
    /// <returns></returns>
    public static IPipelineMiddleware<TContext, TInput, TOutput> CreateMiddleware<TContext, TInput, TOutput>(Func<TContext, TInput, Task<TOutput>> middleware)
        where TContext : IExecutionPipelineContext
    {
        return new DelegatingPipelineMiddleware2<TContext, TInput, TOutput>(middleware);
    }

    /// <summary>
    /// 创建一个基于委托 <paramref name="middleware"/> 的中间件
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="middleware"></param>
    /// <returns></returns>
    public static IPipelineMiddleware<TContext, TInput, TOutput> CreateMiddleware<TContext, TInput, TOutput>(Func<TContext, TInput, PipelineInvokeDelegate<TContext, TOutput>, Task> middleware)
        where TContext : IExecutionPipelineContext
    {
        return new DelegatingPipelineMiddleware3<TContext, TInput, TOutput>(middleware);
    }

    #endregion Middleware

    #region Middleware classes

    private sealed class DelegatingPipelineMiddleware1<TContext, TInput, TOutput>(Func<TInput, Task<TOutput>> middleware)
        : IPipelineMiddleware<TContext, TInput, TOutput>
        where TContext : IExecutionPipelineContext
    {
        #region Public 方法

        [DebuggerStepThrough]
        [StackTraceHidden]
        public async Task InvokeAsync(TContext context, TInput input, PipelineInvokeDelegate<TContext, TOutput> next)
        {
            var middlewareResult = await middleware(input).ConfigureAwait(false);
            await next(context, middlewareResult).ConfigureAwait(false);
        }

        #endregion Public 方法
    }

    private sealed class DelegatingPipelineMiddleware2<TContext, TInput, TOutput>(Func<TContext, TInput, Task<TOutput>> middleware)
        : IPipelineMiddleware<TContext, TInput, TOutput>
        where TContext : IExecutionPipelineContext
    {
        #region Public 方法

        [DebuggerStepThrough]
        [StackTraceHidden]
        public async Task InvokeAsync(TContext context, TInput input, PipelineInvokeDelegate<TContext, TOutput> next)
        {
            var result = await middleware(context, input).ConfigureAwait(false);
            await next(context, result).ConfigureAwait(false);
        }

        #endregion Public 方法
    }

    private sealed class DelegatingPipelineMiddleware3<TContext, TInput, TOutput>(Func<TContext, TInput, PipelineInvokeDelegate<TContext, TOutput>, Task> middleware)
        : IPipelineMiddleware<TContext, TInput, TOutput>
        where TContext : IExecutionPipelineContext
    {
        #region Public 方法

        [DebuggerStepThrough]
        [StackTraceHidden]
        public Task InvokeAsync(TContext context, TInput input, PipelineInvokeDelegate<TContext, TOutput> next)
        {
            return middleware(context, input, next);
        }

        #endregion Public 方法
    }

    #endregion Middleware classes

    #region Endpoint classes

    private sealed class DelegatingPipelineEndpoint1<TContext, TInput>(Func<TInput, Task> middleware)
        : IPipelineMiddleware<TContext, TInput, object?>
        where TContext : IExecutionPipelineContext
    {
        #region Public 方法

        [DebuggerStepThrough]
        [StackTraceHidden]
        public Task InvokeAsync(TContext context, TInput input, PipelineInvokeDelegate<TContext, object?> next)
        {
            return middleware(input);
        }

        #endregion Public 方法
    }

    private sealed class DelegatingPipelineEndpoint2<TContext, TInput>(Func<TContext, TInput, Task> middleware)
        : IPipelineMiddleware<TContext, TInput, object?>
        where TContext : IExecutionPipelineContext
    {
        #region Public 方法

        [DebuggerStepThrough]
        [StackTraceHidden]
        public Task InvokeAsync(TContext context, TInput input, PipelineInvokeDelegate<TContext, object?> next)
        {
            return middleware(context, input);
        }

        #endregion Public 方法
    }

    #endregion Endpoint classes
}
