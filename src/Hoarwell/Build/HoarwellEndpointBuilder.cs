using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Hoarwell.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hoarwell.Build;

/// <summary>
/// Hoarwell 终结点构建器
/// </summary>
/// <typeparam name="TContext"></typeparam>
public class HoarwellEndpointBuilder<TContext>
    where TContext : IHoarwellContext
{
    #region Public 属性

    /// <summary>
    /// Hoarwell 构建器
    /// </summary>
    public HoarwellBuilder HoarwellBuilder { get; }

    #endregion Public 属性

    #region Internal 属性

    internal Dictionary<Type, HandleInboundMessageDelegate> HandleInboundMessageDelegateMap { get; } = [];

    #endregion Internal 属性

    #region Public 构造函数

    /// <inheritdoc cref="HoarwellEndpointBuilder{TContext}"/>
    public HoarwellEndpointBuilder(HoarwellBuilder hoarwellBuilder)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(hoarwellBuilder);

        HoarwellBuilder = hoarwellBuilder;
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <summary>
    /// 构建到 <paramref name="services"/> 中
    /// </summary>
    /// <param name="services"></param>
    public void Build(IServiceCollection services)
    {
        var handleInboundMessageDelegateMap = HandleInboundMessageDelegateMap;

        services.AddOptions<DefaultInboundMessageHandleOptions>(HoarwellBuilder.ApplicationName)
                .Configure(options =>
                {
                    options.HandleInboundMessageDelegateMap = handleInboundMessageDelegateMap;
                });
    }

    /// <summary>
    /// 添加消息 <typeparamref name="TMessage"/> 对应的处理器 <typeparamref name="TMessageHandelr"/>
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TMessageHandelr"></typeparam>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public HoarwellEndpointBuilder<TContext> Handle<TMessage, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMessageHandelr>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TMessageHandelr : IEndpointMessageHandler<TMessage>
    {
        HandleInboundMessageDelegateMap.Add(typeof(TMessage), EndpointMessageHandleHelper.HandleMessageAsync<TMessage, TMessageHandelr>);
        HoarwellBuilder.Services.TryAdd(ServiceDescriptor.DescribeKeyed(typeof(TMessageHandelr), HoarwellBuilder.ApplicationName, typeof(TMessageHandelr), lifetime));
        return this;
    }

    /// <summary>
    /// 添加消息 <typeparamref name="TMessage"/> 对应的处理器 <paramref name="handelr"/>
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TMessageHandelr"></typeparam>
    /// <param name="handelr"></param>
    /// <returns></returns>
    public HoarwellEndpointBuilder<TContext> Handle<TMessage, TMessageHandelr>(TMessageHandelr handelr)
        where TMessageHandelr : IEndpointMessageHandler<TMessage>
    {
        ArgumentNullExceptionHelper.ThrowIfNull(handelr, nameof(handelr));

        HandleInboundMessageDelegateMap.Add(typeof(TMessage), [DebuggerStepThrough][StackTraceHidden] (IHoarwellContext context, InboundMetadata input) => handelr.HandleAsync(context, (TMessage?)input.Value));
        return this;
    }

    #endregion Public 方法
}
