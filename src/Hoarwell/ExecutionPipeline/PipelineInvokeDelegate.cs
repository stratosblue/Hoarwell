namespace Hoarwell.ExecutionPipeline;

/// <summary>
/// 中间件执行委托
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TInput"></typeparam>
/// <param name="context"></param>
/// <param name="input"></param>
/// <returns></returns>
public delegate Task PipelineInvokeDelegate<in TContext, in TInput>(TContext context, TInput input) where TContext : IExecutionPipelineContext;
