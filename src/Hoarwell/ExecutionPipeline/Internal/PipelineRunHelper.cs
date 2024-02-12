using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Hoarwell.ExecutionPipeline.Internal;

internal static class PipelineRunHelper
{
    #region Public 方法

    public static PipelineInvokeDelegate<TContext, TInput> BuildInvokeDelegate<TMiddleware, TContext, TInput, TOutput>(TMiddleware middleware, object? unTypedNext)
        where TMiddleware : IPipelineMiddleware<TContext, TInput, TOutput>
        where TContext : IExecutionPipelineContext
    {
        var next = unTypedNext is null
                   ? [DebuggerStepThrough][StackTraceHidden] static (context, input) => throw new InvalidOperationException($"The data \"{input}\" has reached the end of pipeline. Please check the pipeline has correctly configured endpoint.")
                   : (PipelineInvokeDelegate<TContext, TOutput>)unTypedNext;

        return [DebuggerStepThrough][StackTraceHidden] (context, input) => middleware.InvokeAsync(context, input, next);
    }

    public static PipelineInvokeDelegate<TContext, TInput> BuildInvokeDelegate<TMiddleware, TContext, TInput, TOutput>(object? unTypedNext, object? serviceKey)
        where TMiddleware : IPipelineMiddleware<TContext, TInput, TOutput>
        where TContext : IExecutionPipelineContext
    {
        var next = unTypedNext is null
                   ? [DebuggerStepThrough][StackTraceHidden] static (context, input) => throw new InvalidOperationException($"The data \"{input}\" has reached the end of pipeline. Please check the pipeline has correctly configured endpoint.")
                   : (PipelineInvokeDelegate<TContext, TOutput>)unTypedNext;

        if (serviceKey is null)
        {
            return [DebuggerStepThrough][StackTraceHidden] (context, input) =>
            {
                var middleware = context.Services.GetService(typeof(TMiddleware))
                                 ?? throw new InvalidOperationException($"Can not get {typeof(TMiddleware)} from context's service provider");

                return ((TMiddleware)middleware).InvokeAsync(context, input, next);
            };
        }
        else
        {
            return [DebuggerStepThrough][StackTraceHidden] (context, input) =>
            {
                var middleware = context.Services.GetKeyedService<TMiddleware>(serviceKey)
                                 ?? throw new InvalidOperationException($"Can not get {typeof(TMiddleware)} from context's service provider");

                return ((TMiddleware)middleware).InvokeAsync(context, input, next);
            };
        }
    }

    public static object BuildStartPipelineInvokeDelegate<TContext, TOutput>(object? unTypedNext)
        where TContext : IExecutionPipelineContext
    {
        ArgumentNullExceptionHelper.ThrowIfNull(unTypedNext);

        var next = (PipelineInvokeDelegate<TContext, TOutput>)unTypedNext;

        PipelineInvokeDelegate<TContext, TOutput> pipelineInvokeDelegate = [DebuggerStepThrough][StackTraceHidden] (context, input) => StartPipeline(context, input, next);

        return pipelineInvokeDelegate;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task StartPipeline<TContext, TOutput>(TContext context, TOutput input, PipelineInvokeDelegate<TContext, TOutput> next)
        where TContext : IExecutionPipelineContext
    {
        return context.Scheduler is null
               ? next(context, input)
               : StartPipelineWithScheduler(context, input, next);
    }

    #endregion Public 方法

    #region Private 方法

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Task StartPipelineWithScheduler<TContext, TOutput>(TContext context, TOutput input, PipelineInvokeDelegate<TContext, TOutput> next)
        where TContext : IExecutionPipelineContext
    {
        return Task.Factory.StartNew(function: [DebuggerStepThrough][StackTraceHidden] () => next(context, input),
                                     cancellationToken: context.ExecutionAborted,
                                     creationOptions: TaskCreationOptions.None,
                                     scheduler: context.Scheduler!)
                           .Unwrap();
    }

    #endregion Private 方法
}
