#pragma warning disable IDE0130

using System.ComponentModel;
using Hoarwell;
using Hoarwell.Build;
using Hoarwell.Features;
using Hoarwell.Middlewares;
using Hoarwell.Options.Features;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Hoarwell.Features
{
    /// <summary>
    ///
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class InboundOutboundIdleStateFeatureExtensions
    {
        #region Public 方法

        /// <summary>
        /// 获取最后的活跃时间
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static DateTime? GetLastActiveTime(this IInboundOutboundIdleStateFeature feature)
        {
            if (feature.LastInbound.HasValue
                || feature.LastOutbound.HasValue)
            {
                if (feature.LastInbound.HasValue
                    ^ feature.LastOutbound.HasValue)
                {
                    return feature.LastInbound ?? feature.LastOutbound;
                }

                return feature.LastInbound!.Value > feature.LastOutbound!.Value
                       ? feature.LastInbound
                       : feature.LastOutbound;
            }

            return null;
        }

        #endregion Public 方法
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class InboundOutboundIdleStateFeatureDIExtensions
    {
        #region Public 方法

        /// <summary>
        /// 添加默认的入站空闲状态检查，超时时间为 <paramref name="inboundIdleTimeout"/>
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="chainNode"></param>
        /// <param name="inboundIdleTimeout"></param>
        /// <returns></returns>
        public static InboundPipelineBuilderChainNode<TContext, TInput> UseInboundIdleStateHandler<TContext, TInput>(this InboundPipelineBuilderChainNode<TContext, TInput> chainNode, TimeSpan inboundIdleTimeout)
            where TContext : IHoarwellContext
        {
            return chainNode.UseInboundIdleStateHandler<TContext, TInput, InboundOutboundIdleStateFeature>(inboundIdleTimeout);
        }

        /// <summary>
        /// 添加基于 <typeparamref name="TInboundOutboundIdleStateFeature"/> 的入站空闲状态检查，超时时间为 <paramref name="inboundIdleTimeout"/>
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TInboundOutboundIdleStateFeature"></typeparam>
        /// <param name="chainNode"></param>
        /// <param name="inboundIdleTimeout"></param>
        /// <returns></returns>
        public static InboundPipelineBuilderChainNode<TContext, TInput> UseInboundIdleStateHandler<TContext, TInput, TInboundOutboundIdleStateFeature>(this InboundPipelineBuilderChainNode<TContext, TInput> chainNode, TimeSpan inboundIdleTimeout)
            where TContext : IHoarwellContext
            where TInboundOutboundIdleStateFeature : class, IInboundOutboundIdleStateFeature
        {
            chainNode.HoarwellBuilder.AddInboundIdleStateHandleCore<TInboundOutboundIdleStateFeature>().Configure(m => m.InboundIdleTimeout = inboundIdleTimeout);
            return chainNode.Use<DefaultInboundIdleStateHandleMiddleware<TContext, TInput, TInboundOutboundIdleStateFeature>, TInput>(ServiceLifetime.Scoped);
        }

        /// <summary>
        /// 添加默认的出站空闲状态检查，超时时间为 <paramref name="outboundIdleTimeout"/>
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="chainNode"></param>
        /// <param name="outboundIdleTimeout"></param>
        /// <returns></returns>
        public static OutboundPipelineBuilderChainNode<TContext, TOutput> UseOutboundIdleStateHandler<TContext, TOutput>(this OutboundPipelineBuilderChainNode<TContext, TOutput> chainNode, TimeSpan outboundIdleTimeout)
            where TContext : IHoarwellContext
        {
            return chainNode.UseOutboundIdleStateHandler<TContext, TOutput, InboundOutboundIdleStateFeature>(outboundIdleTimeout);
        }

        /// <summary>
        /// 添加基于 <typeparamref name="TInboundOutboundIdleStateFeature"/> 的出站空闲状态检查，超时时间为 <paramref name="outboundIdleTimeout"/>
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <typeparam name="TInboundOutboundIdleStateFeature"></typeparam>
        /// <param name="chainNode"></param>
        /// <param name="outboundIdleTimeout"></param>
        /// <returns></returns>
        public static OutboundPipelineBuilderChainNode<TContext, TOutput> UseOutboundIdleStateHandler<TContext, TOutput, TInboundOutboundIdleStateFeature>(this OutboundPipelineBuilderChainNode<TContext, TOutput> chainNode, TimeSpan outboundIdleTimeout)
            where TContext : IHoarwellContext
            where TInboundOutboundIdleStateFeature : class, IInboundOutboundIdleStateFeature
        {
            chainNode.HoarwellBuilder.AddInboundIdleStateHandleCore<TInboundOutboundIdleStateFeature>().Configure(m => m.OutboundIdleTimeout = outboundIdleTimeout);
            return chainNode.Use<DefaultOutboundIdleStateHandleMiddleware<TContext, TOutput, TInboundOutboundIdleStateFeature>, TOutput>(ServiceLifetime.Scoped);
        }

        #endregion Public 方法

        #region Internal 方法

        internal static OptionsBuilder<InboundOutboundIdleOptions> AddInboundIdleStateHandleCore<TInboundOutboundIdleStateFeature>(this HoarwellBuilder builder)
            where TInboundOutboundIdleStateFeature : class, IInboundOutboundIdleStateFeature
        {
            builder.Services.AddHoarwellContextAccessor();
            builder.Services.TryAddKeyedScoped<TInboundOutboundIdleStateFeature>(builder.ApplicationName);
            return builder.Services.AddOptions<InboundOutboundIdleOptions>(builder.ApplicationName);
        }

        #endregion Internal 方法
    }
}
