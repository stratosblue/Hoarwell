using Hoarwell.ExecutionPipeline;

namespace Hoarwell.Middlewares;

/// <summary>
/// 管道终结点
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TInput"></typeparam>
public abstract class PipelineEndpoint<TContext, TInput> : IPipelineMiddleware<TContext, TInput, object?>
    where TContext : IHoarwellContext
{
    #region Public 方法

    /// <inheritdoc/>
    public Task InvokeAsync(TContext context, TInput input, PipelineInvokeDelegate<TContext, object?> next) => InvokeAsync(context, input);

    /// <inheritdoc cref="InvokeAsync(TContext, TInput, PipelineInvokeDelegate{TContext, object?})"/>
    public abstract Task InvokeAsync(TContext context, TInput input);

    #endregion Public 方法
}
