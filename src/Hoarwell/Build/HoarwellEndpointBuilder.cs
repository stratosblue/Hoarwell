using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Hoarwell.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hoarwell.Build;

/// <summary>
/// Hoarwell 终结点构建器
/// </summary>
public class HoarwellEndpointBuilder
{
    #region Public 属性

    /// <summary>
    /// Hoarwell 构建器
    /// </summary>
    public HoarwellBuilder HoarwellBuilder { get; }

    #endregion Public 属性

    #region Internal 属性

    internal Dictionary<Type, HandleInboundMessageDelegate> HandleInboundMessageDelegateMap { get; } = [];

    internal HandleInboundMessageDelegate? UnhandledCatchDelegate { get; private set; }

    #endregion Internal 属性

    #region Public 构造函数

    /// <inheritdoc cref="HoarwellEndpointBuilder"/>
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
        var unhandledCatchDelegate = UnhandledCatchDelegate;

        services.AddOptions<DefaultInboundMessageHandleOptions>(HoarwellBuilder.ApplicationName)
                .Configure(options =>
                {
                    options.HandleInboundMessageDelegateMap = handleInboundMessageDelegateMap;
                    options.UnhandledCatchDelegate = unhandledCatchDelegate;
                });
    }

    /// <summary>
    /// 添加消息 <typeparamref name="TMessage"/> 对应的处理器 <typeparamref name="TMessageHandler"/>
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TMessageHandler"></typeparam>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public HoarwellEndpointBuilder Handle<TMessage, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMessageHandler>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TMessageHandler : IEndpointMessageHandler<TMessage>
    {
        HandleInboundMessageDelegateMap.Add(typeof(TMessage), EndpointMessageHandleHelper.HandleMessageAsync<TMessage, TMessageHandler>);
        HoarwellBuilder.Services.TryAdd(ServiceDescriptor.DescribeKeyed(typeof(TMessageHandler), HoarwellBuilder.ApplicationName, typeof(TMessageHandler), lifetime));
        return this;
    }

    /// <summary>
    /// 添加消息 <typeparamref name="TMessage"/> 对应的处理器 <paramref name="handler"/>
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TMessageHandler"></typeparam>
    /// <param name="handler"></param>
    /// <returns></returns>
    public HoarwellEndpointBuilder Handle<TMessage, TMessageHandler>(TMessageHandler handler)
        where TMessageHandler : IEndpointMessageHandler<TMessage>
    {
        ArgumentNullExceptionHelper.ThrowIfNull(handler, nameof(handler));

        HandleInboundMessageDelegateMap.Add(typeof(TMessage), [DebuggerStepThrough][StackTraceHidden] (IHoarwellContext context, InboundMetadata input) => handler.HandleAsync(context, (TMessage?)input.Value));
        return this;
    }

    #region Unhandled

    /// <summary>
    /// 使用处理器 <typeparamref name="TMessageHandler"/> 捕获所有未处理的消息
    /// </summary>
    /// <typeparam name="TMessageHandler"></typeparam>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public HoarwellEndpointBuilder CatchUnhandled<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMessageHandler>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TMessageHandler : IEndpointMessageHandler<object>
    {
        UnhandledCatchDelegate = EndpointMessageHandleHelper.HandleMessageAsync<object, TMessageHandler>;
        HoarwellBuilder.Services.TryAdd(ServiceDescriptor.DescribeKeyed(typeof(TMessageHandler), HoarwellBuilder.ApplicationName, typeof(TMessageHandler), lifetime));
        return this;
    }

    /// <summary>
    /// 使用处理器 <paramref name="handler"/> 捕获所有未处理的消息
    /// </summary>
    /// <typeparam name="TMessageHandler"></typeparam>
    /// <param name="handler"></param>
    /// <returns></returns>
    public HoarwellEndpointBuilder CatchUnhandled<TMessageHandler>(TMessageHandler handler)
        where TMessageHandler : IEndpointMessageHandler<object>
    {
        ArgumentNullExceptionHelper.ThrowIfNull(handler, nameof(handler));

        UnhandledCatchDelegate = [DebuggerStepThrough][StackTraceHidden] (IHoarwellContext context, InboundMetadata input) => handler.HandleAsync(context, input.Value);
        return this;
    }

    #endregion Unhandled

    #endregion Public 方法
}
