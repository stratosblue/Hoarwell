using Hoarwell.ExecutionPipeline;

namespace Hoarwell.Middlewares;

/// <summary>
/// 输入转换中间件，接收<typeparamref name="TInput"/>, 并传递<typeparamref name="TOutput"/>给下一个中间件
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TOutput"></typeparam>
public abstract class TransformMiddleware<TContext, TInput, TOutput> : IPipelineMiddleware<TContext, TInput, TOutput>
    where TContext : IHoarwellContext
{
    #region Public 方法

    /// <inheritdoc/>
    public virtual async Task InvokeAsync(TContext context, TInput input, PipelineInvokeDelegate<TContext, TOutput> next)
    {
        var output = await InvokeAsync(context, input).ConfigureAwait(false);
        await next(context, output).ConfigureAwait(false);
    }

    /// <summary>
    /// 执行并返回传递给下一个中间件的 <typeparamref name="TOutput"/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public abstract Task<TOutput> InvokeAsync(TContext context, TInput input);

    #endregion Public 方法
}
