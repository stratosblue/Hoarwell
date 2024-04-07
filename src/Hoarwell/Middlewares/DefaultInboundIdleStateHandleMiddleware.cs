using Hoarwell.ExecutionPipeline;
using Hoarwell.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Hoarwell.Middlewares;

internal sealed class DefaultInboundIdleStateHandleMiddleware<TContext, TInput, TInboundOutboundIdleStateFeature>([ServiceKey] string applicationName)
    : IPipelineMiddleware<TContext, TInput, TInput>
    where TContext : IHoarwellContext
    where TInboundOutboundIdleStateFeature : IInboundOutboundIdleStateFeature
{
    #region Private 字段

    private TInboundOutboundIdleStateFeature? _inboundOutboundIdleStateFeature;

    #endregion Private 字段

    #region Public 方法

    public Task InvokeAsync(TContext context, TInput input, PipelineInvokeDelegate<TContext, TInput> next)
    {
        if (_inboundOutboundIdleStateFeature is null)
        {
            _inboundOutboundIdleStateFeature = context.Services.GetRequiredKeyedService<TInboundOutboundIdleStateFeature>(applicationName);
            context.Features.Set<IInboundOutboundIdleStateFeature>(_inboundOutboundIdleStateFeature);
        }

        _inboundOutboundIdleStateFeature.UpdateInboundTime();

        return next(context, input);
    }

    #endregion Public 方法
}
