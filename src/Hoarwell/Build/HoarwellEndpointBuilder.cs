﻿using System.Diagnostics;
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
    /// 添加消息 <typeparamref name="TMessage"/> 对应的处理器 <typeparamref name="TMessageHandelr"/>
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TMessageHandelr"></typeparam>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public HoarwellEndpointBuilder Handle<TMessage, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMessageHandelr>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
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
    public HoarwellEndpointBuilder Handle<TMessage, TMessageHandelr>(TMessageHandelr handelr)
        where TMessageHandelr : IEndpointMessageHandler<TMessage>
    {
        ArgumentNullExceptionHelper.ThrowIfNull(handelr, nameof(handelr));

        HandleInboundMessageDelegateMap.Add(typeof(TMessage), [DebuggerStepThrough][StackTraceHidden] (IHoarwellContext context, InboundMetadata input) => handelr.HandleAsync(context, (TMessage?)input.Value));
        return this;
    }

    #region Unhandled

    /// <summary>
    /// 使用处理器 <typeparamref name="TMessageHandelr"/> 捕获所有未处理的消息
    /// </summary>
    /// <typeparam name="TMessageHandelr"></typeparam>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public HoarwellEndpointBuilder CatchUnhandled<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMessageHandelr>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TMessageHandelr : IEndpointMessageHandler<object>
    {
        UnhandledCatchDelegate = EndpointMessageHandleHelper.HandleMessageAsync<object, TMessageHandelr>;
        HoarwellBuilder.Services.TryAdd(ServiceDescriptor.DescribeKeyed(typeof(TMessageHandelr), HoarwellBuilder.ApplicationName, typeof(TMessageHandelr), lifetime));
        return this;
    }

    /// <summary>
    /// 使用处理器 <paramref name="handelr"/> 捕获所有未处理的消息
    /// </summary>
    /// <typeparam name="TMessageHandelr"></typeparam>
    /// <param name="handelr"></param>
    /// <returns></returns>
    public HoarwellEndpointBuilder CatchUnhandled<TMessageHandelr>(TMessageHandelr handelr)
        where TMessageHandelr : IEndpointMessageHandler<object>
    {
        ArgumentNullExceptionHelper.ThrowIfNull(handelr, nameof(handelr));

        UnhandledCatchDelegate = [DebuggerStepThrough][StackTraceHidden] (IHoarwellContext context, InboundMetadata input) => handelr.HandleAsync(context, input.Value);
        return this;
    }

    #endregion Unhandled

    #endregion Public 方法
}
