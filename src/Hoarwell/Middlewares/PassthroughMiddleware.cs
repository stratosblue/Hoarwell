using Hoarwell.ExecutionPipeline;

namespace Hoarwell.Middlewares;

/// <summary>
/// 不改变 <typeparamref name="TInput"/> 的中间件
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TInput"></typeparam>
public abstract class PassthroughMiddleware<TContext, TInput> : IPipelineMiddleware<TContext, TInput, TInput>
    where TContext : IHoarwellContext
{
    #region Public 方法

    /// <inheritdoc/>
    public abstract Task InvokeAsync(TContext context, TInput input, PipelineInvokeDelegate<TContext, TInput> next);

    #endregion Public 方法
}
