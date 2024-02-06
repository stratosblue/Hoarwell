using System.Diagnostics.CodeAnalysis;
using Hoarwell.ExecutionPipeline;
using Hoarwell.ExecutionPipeline.Build;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hoarwell.Build;

// 隐藏 ExecutionPipeline 的存在

/// <summary>
/// 入站管道构建器
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TInput"></typeparam>
public class InboundPipelineBuilder<TContext, TInput>
    : InboundPipelineBuilderChainNode<TContext, TInput>
    where TContext : IHoarwellContext
{
    #region Private 字段

    private readonly IExecutionPipelineBuilder<TContext, TInput> _innerBuilder;

    #endregion Private 字段

    #region Internal 构造函数

    internal InboundPipelineBuilder(HoarwellBuilder hoarwellBuilder, IExecutionPipelineBuilder<TContext, TInput> innerBuilder) : base(hoarwellBuilder, innerBuilder)
    {
        _innerBuilder = innerBuilder;
    }

    #endregion Internal 构造函数

    #region Internal 方法

    internal PipelineInvokeDelegate<TContext, TInput> Build()
    {
        return _innerBuilder.Build();
    }

    #endregion Internal 方法
}

/// <summary>
/// 入站管道构建器节点
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TInput"></typeparam>
public class InboundPipelineBuilderChainNode<TContext, TInput>
    where TContext : IHoarwellContext
{
    #region Private 字段

    private readonly IExecutionPipelineBuilderChainNode<TContext, TInput> _innerChainNode;

    #endregion Private 字段

    #region Internal 构造函数

    /// <summary>
    /// Hoarwell 构建器
    /// </summary>
    public HoarwellBuilder HoarwellBuilder { get; }

    internal InboundPipelineBuilderChainNode(HoarwellBuilder hoarwellBuilder, IExecutionPipelineBuilderChainNode<TContext, TInput> innerChainNode)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(hoarwellBuilder);
        ArgumentNullExceptionHelper.ThrowIfNull(innerChainNode);

        HoarwellBuilder = hoarwellBuilder;
        _innerChainNode = innerChainNode;
    }

    #endregion Internal 构造函数

    #region Public 方法

    /// <summary>
    /// 添加管道处理中间件 <typeparamref name="TMiddleware"/>
    /// </summary>
    /// <typeparam name="TMiddleware"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public InboundPipelineBuilderChainNode<TContext, TOutput> Use<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMiddleware, TOutput>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TMiddleware : class, IPipelineMiddleware<TContext, TInput, TOutput>
    {
        var serviceDescriptor = ServiceDescriptor.DescribeKeyed(typeof(TMiddleware), HoarwellBuilder.ApplicationName, typeof(TMiddleware), lifetime);

        HoarwellBuilder.Services.TryAdd(serviceDescriptor);

        return new InboundPipelineBuilderChainNode<TContext, TOutput>(HoarwellBuilder, _innerChainNode.Use<TMiddleware, TOutput>(HoarwellBuilder.ApplicationName));
    }

    /// <summary>
    /// 添加管道处理中间件 <paramref name="middleware"/>
    /// </summary>
    /// <typeparam name="TMiddleware"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="middleware"></param>
    /// <returns></returns>
    public InboundPipelineBuilderChainNode<TContext, TOutput> Use<TMiddleware, TOutput>(TMiddleware middleware) where TMiddleware : class, IPipelineMiddleware<TContext, TInput, TOutput>
    {
        return new InboundPipelineBuilderChainNode<TContext, TOutput>(HoarwellBuilder, _innerChainNode.Use<TMiddleware, TOutput>(middleware));
    }

    /// <summary>
    /// 添加管道处理中间件 <typeparamref name="TMiddleware"/>
    /// </summary>
    /// <typeparam name="TMiddleware"></typeparam>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public InboundPipelineBuilderChainNode<TContext, TInput> Use<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMiddleware>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TMiddleware : class, IPipelineMiddleware<TContext, TInput, TInput>
    {
        return Use<TMiddleware, TInput>(lifetime);
    }

    /// <summary>
    /// 添加管道处理中间件 <paramref name="middleware"/>
    /// </summary>
    /// <typeparam name="TMiddleware"></typeparam>
    /// <param name="middleware"></param>
    /// <returns></returns>
    public InboundPipelineBuilderChainNode<TContext, TInput> Use<TMiddleware>(TMiddleware middleware) where TMiddleware : class, IPipelineMiddleware<TContext, TInput, TInput>
    {
        return new InboundPipelineBuilderChainNode<TContext, TInput>(HoarwellBuilder, _innerChainNode.Use<TMiddleware, TInput>(middleware));
    }

    #endregion Public 方法
}
