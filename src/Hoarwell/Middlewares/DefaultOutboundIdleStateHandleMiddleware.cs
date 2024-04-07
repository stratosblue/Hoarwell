using Hoarwell.ExecutionPipeline;
using Hoarwell.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Hoarwell.Middlewares;

internal sealed class DefaultOutboundIdleStateHandleMiddleware<TContext, TOutput, TInboundOutboundIdleStateFeature>([ServiceKey] string applicationName)
    : IPipelineMiddleware<TContext, TOutput, TOutput>
    where TContext : IHoarwellContext
    where TInboundOutboundIdleStateFeature : IInboundOutboundIdleStateFeature
{
    #region Private 字段

    private TInboundOutboundIdleStateFeature? _inboundOutboundIdleStateFeature;

    #endregion Private 字段

    #region Public 方法

    public Task InvokeAsync(TContext context, TOutput input, PipelineInvokeDelegate<TContext, TOutput> next)
    {
        if (_inboundOutboundIdleStateFeature is null)
        {
            _inboundOutboundIdleStateFeature = context.Services.GetRequiredKeyedService<TInboundOutboundIdleStateFeature>(applicationName);
            context.Features.Set<IInboundOutboundIdleStateFeature>(_inboundOutboundIdleStateFeature);
        }

        _inboundOutboundIdleStateFeature.UpdateOutboundTime();

        return next(context, input);
    }

    #endregion Public 方法
}
