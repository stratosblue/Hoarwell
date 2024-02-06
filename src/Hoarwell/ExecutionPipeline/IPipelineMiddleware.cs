namespace Hoarwell.ExecutionPipeline;

/// <summary>
/// 执行管道中间件
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TOutput"></typeparam>
public interface IPipelineMiddleware<TContext, in TInput, out TOutput>
    where TContext : IExecutionPipelineContext
{
    #region Public 方法

    /// <summary>
    /// 中间件执行逻辑
    /// </summary>
    /// <param name="context">上下文</param>
    /// <param name="input">输入</param>
    /// <param name="next">后续中间件执行委托</param>
    /// <returns></returns>
    Task InvokeAsync(TContext context, TInput input, PipelineInvokeDelegate<TContext, TOutput> next);

    #endregion Public 方法
}
