using System.Buffers;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using Hoarwell;
using Hoarwell.Build;
using Hoarwell.ExecutionPipeline.Build;
using Hoarwell.Middlewares;
using Hoarwell.Options;
using Hoarwell.Outputters;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Hoarwell ServiceCollection 构建拓展
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class HoarwellServiceCollectionExtensions
{
    #region Public 方法

    /// <summary>
    /// 添加 Hoarwell 并开始构建
    /// </summary>
    /// <param name="services"></param>
    /// <param name="applicationName">应用程序名称</param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static HoarwellBuilder AddHoarwell(this IServiceCollection services, string applicationName, Action<HoarwellOptions>? configureOptions = null)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(services);
        ArgumentNullExceptionHelper.ThrowIfNull(applicationName);

        services.AddOptions<HoarwellOptions>();

        if (configureOptions is not null)
        {
            services.Configure(applicationName, configureOptions);
        }

        var streamOutputterAdapterServiceDescriptor = ServiceDescriptor.DescribeKeyed(serviceType: typeof(IOutputterAdapter<Stream>),
                                                                                      serviceKey: KeyedService.AnyKey,
                                                                                      implementationFactory: static (_, _) => StreamOutputterAdapter.Shared,
                                                                                      lifetime: ServiceLifetime.Singleton);
        services.TryAdd(streamOutputterAdapterServiceDescriptor);

        var pipeWriterOutputterAdapterServiceDescriptor = ServiceDescriptor.DescribeKeyed(serviceType: typeof(IOutputterAdapter<PipeWriter>),
                                                                                          serviceKey: KeyedService.AnyKey,
                                                                                          implementationFactory: static (_, _) => PipeWriterOutputterAdapter.Shared,
                                                                                          lifetime: ServiceLifetime.Singleton);
        services.TryAdd(pipeWriterOutputterAdapterServiceDescriptor);

        return new HoarwellBuilder(services, applicationName);
    }

    #region Application

    /// <summary>
    /// 使用应用程序 <typeparamref name="TApplication"/>. 该应用程序上下文类型为 <typeparamref name="TContext"/>, 输入器类型为 <typeparamref name="TInputter"/>, 输出器类型为 <typeparamref name="TOutputter"/>
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TApplication"></typeparam>
    /// <typeparam name="TInputter"></typeparam>
    /// <typeparam name="TOutputter"></typeparam>
    /// <param name="builder"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public static HoarwellBuilder<TContext, TInputter, TOutputter> UseApplication<TContext, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApplication, TInputter, TOutputter>(this HoarwellBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TContext : IHoarwellContext
        where TApplication : IHoarwellApplication<TContext, TInputter, TOutputter>
    {
        ArgumentNullExceptionHelper.ThrowIfNull(builder);

        var services = builder.Services;

        services.TryAdd(ServiceDescriptor.DescribeKeyed(typeof(TApplication), serviceKey: builder.ApplicationName, typeof(TApplication), lifetime));
        services.TryAdd(ServiceDescriptor.DescribeKeyed(typeof(IHoarwellApplication<TContext, TInputter, TOutputter>), builder.ApplicationName, typeof(TApplication), lifetime));

        services.TryAdd(ServiceDescriptor.DescribeKeyed(typeof(IHoarwellApplicationRunner), builder.ApplicationName, typeof(DefaultHoarwellApplicationRunner<TContext, TApplication, TInputter, TOutputter>), lifetime));

        return new HoarwellBuilder<TContext, TInputter, TOutputter>(builder);
    }

    /// <summary>
    /// 使用默认应用程序 <see cref="DefaultHoarwellApplication{TInputter, TOutputter}"/>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public static HoarwellBuilder<HoarwellContext, PipeReader, PipeWriter> UseDefaultApplication(this HoarwellBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        return builder.UseApplication<HoarwellContext, DefaultHoarwellApplication<PipeReader, PipeWriter>, PipeReader, PipeWriter>(lifetime);
    }

    /// <summary>
    /// 使用默认应用程序 <see cref="DefaultHoarwellApplication{TInputter, TOutputter}"/>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public static HoarwellBuilder<HoarwellContext, Stream, Stream> UseDefaultStreamApplication(this HoarwellBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        return builder.UseApplication<HoarwellContext, DefaultHoarwellApplication<Stream, Stream>, Stream, Stream>(lifetime);
    }

    #endregion Application

    #region Inbound

    /// <summary>
    /// 配置入站管道
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInputter"></typeparam>
    /// <typeparam name="TOutputter"></typeparam>
    /// <param name="builder"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static HoarwellBuilder<TContext, TInputter, TOutputter> ConfigureInboundPipeline<TContext, TInputter, TOutputter>(this HoarwellBuilder<TContext, TInputter, TOutputter> builder,
                                                                                                                             Action<InboundPipelineBuilder<TContext, TInputter>> setupAction)
        where TContext : IHoarwellContext
    {
        ArgumentNullExceptionHelper.ThrowIfNull(builder);
        ArgumentNullExceptionHelper.ThrowIfNull(setupAction);

        var services = builder.Services;

        var inboundPipelineBuilder = new InboundPipelineBuilder<TContext, TInputter>(builder, ExecutionPipelineBuilder.Create<TContext, TInputter>());
        setupAction(inboundPipelineBuilder);

        var inboundPipelineInvokeDelegate = inboundPipelineBuilder.Build();

        services.AddOptions<InboundPipelineOptions<TContext, TInputter>>(builder.ApplicationName)
                .Configure(options => options.InboundPipelineInvokeDelegate = inboundPipelineInvokeDelegate);

        return builder;
    }

    /// <summary>
    /// 运行入站终结点
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="chainNode"></param>
    /// <param name="setupAction"></param>
    public static void RunEndpoint<TContext>(this InboundPipelineBuilderChainNode<TContext, InboundMetadata> chainNode,
                                             Action<HoarwellEndpointBuilder> setupAction)
        where TContext : IHoarwellContext
    {
        ArgumentNullExceptionHelper.ThrowIfNull(chainNode);
        ArgumentNullExceptionHelper.ThrowIfNull(setupAction);

        var builder = new HoarwellEndpointBuilder(chainNode.HoarwellBuilder);

        setupAction(builder);

        builder.Build(chainNode.HoarwellBuilder.Services);

        chainNode.Use<DefaultInboundEndpoint<TContext>, object?>();
    }

    /// <summary>
    /// 使用默认的消息序列化器
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="chainNode"></param>
    /// <returns></returns>
    public static InboundPipelineBuilderChainNode<TContext, InboundMetadata> UseDefaultMessageDeserializer<TContext>(this InboundPipelineBuilderChainNode<TContext, ReadOnlySequence<byte>> chainNode)
        where TContext : IHoarwellContext
    {
        return chainNode.Use<DefaultInboundSerializeMiddleware<TContext>, InboundMetadata>();
    }

    #endregion Inbound

    #region Outbound

    /// <summary>
    /// 配置出站管道
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInputter"></typeparam>
    /// <typeparam name="TOutputter"></typeparam>
    /// <param name="builder"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static HoarwellBuilder<TContext, TInputter, TOutputter> ConfigureOutboundPipeline<TContext, TInputter, TOutputter>(this HoarwellBuilder<TContext, TInputter, TOutputter> builder,
                                                                                                                              Action<OutboundPipelineBuilder<TContext, OutboundMetadata>> setupAction)
        where TContext : IHoarwellContext
    {
        ArgumentNullExceptionHelper.ThrowIfNull(builder);
        ArgumentNullExceptionHelper.ThrowIfNull(setupAction);

        var services = builder.Services;

        var outboundPipelineBuilder = new OutboundPipelineBuilder<TContext, OutboundMetadata>(builder, ExecutionPipelineBuilder.Create<TContext, OutboundMetadata>());
        setupAction(outboundPipelineBuilder);

        var outboundPipelineInvokeDelegate = outboundPipelineBuilder.Build();

        services.AddOptions<OutboundPipelineOptions<TContext>>(builder.ApplicationName)
                .Configure(options => options.OutboundPipelineInvokeDelegate = outboundPipelineInvokeDelegate);

        return builder;
    }

    /// <summary>
    /// 运行默认的消息序列化器终结点
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="chainNode"></param>
    public static void RunDefaultMessageSerializer<TContext>(this OutboundPipelineBuilderChainNode<TContext, OutboundMetadata> chainNode)
        where TContext : IHoarwellContext
    {
        chainNode.Use<DefaultOutboundSerializeEndpoint<TContext>, object?>();
    }

    #endregion Outbound

    #region Serializer

    /// <summary>
    /// 使用默认的序列化器
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInputter"></typeparam>
    /// <typeparam name="TOutputter"></typeparam>
    /// <param name="builder"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static HoarwellBuilder<TContext, TInputter, TOutputter> UseDefaultSerializer<TContext, TInputter, TOutputter>(this HoarwellBuilder<TContext, TInputter, TOutputter> builder,
                                                                                                                         Action<HoarwellDefaultSerializerBuilder> setupAction)
        where TContext : IHoarwellContext
    {
        ArgumentNullExceptionHelper.ThrowIfNull(builder);
        ArgumentNullExceptionHelper.ThrowIfNull(setupAction);

        var serializerBuilder = new HoarwellDefaultSerializerBuilder(builder);

        setupAction(serializerBuilder);

        serializerBuilder.Build(builder.Services);

        return builder;
    }

    #endregion Serializer

    #region TypeIdentifierAnalyzer

    /// <summary>
    /// 使用默认的类型标识符分析器
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInputter"></typeparam>
    /// <typeparam name="TOutputter"></typeparam>
    /// <param name="builder"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static HoarwellBuilder<TContext, TInputter, TOutputter> UseDefaultTypeIdentifierAnalyzer<TContext, TInputter, TOutputter>(this HoarwellBuilder<TContext, TInputter, TOutputter> builder,
                                                                                                                                     Action<HoarwellDefaultTypeIdentifierAnalyzerBuilder> setupAction)
        where TContext : IHoarwellContext
    {
        ArgumentNullExceptionHelper.ThrowIfNull(setupAction);

        var analyzerBuilder = new HoarwellDefaultTypeIdentifierAnalyzerBuilder(builder);

        setupAction(analyzerBuilder);

        analyzerBuilder.Build(builder.Services);

        UseTypeIdentifierAnalyzer<DefaultTypeIdentifierAnalyzer>(builder);

        return builder;
    }

    /// <summary>
    /// 使用类型标识符分析器 <typeparamref name="TTypeIdentifierAnalyzer"/>
    /// </summary>
    /// <typeparam name="TTypeIdentifierAnalyzer"></typeparam>
    /// <param name="builder"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public static HoarwellBuilder UseTypeIdentifierAnalyzer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTypeIdentifierAnalyzer>(this HoarwellBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TTypeIdentifierAnalyzer : ITypeIdentifierAnalyzer
    {
        ArgumentNullExceptionHelper.ThrowIfNull(builder);

        builder.Services.TryAddEnumerable(ServiceDescriptor.DescribeKeyed(typeof(ITypeIdentifierAnalyzer), builder.ApplicationName, typeof(TTypeIdentifierAnalyzer), lifetime));

        return builder;
    }

    /// <summary>
    /// 使用类型标识符分析器 <typeparamref name="TTypeIdentifierAnalyzer"/>
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TInputter"></typeparam>
    /// <typeparam name="TOutputter"></typeparam>
    /// <typeparam name="TTypeIdentifierAnalyzer"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static HoarwellBuilder<TContext, TInputter, TOutputter> UseTypeIdentifierAnalyzer<TContext, TInputter, TOutputter, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTypeIdentifierAnalyzer>(this HoarwellBuilder<TContext, TInputter, TOutputter> builder)
        where TContext : IHoarwellContext
        where TTypeIdentifierAnalyzer : ITypeIdentifierAnalyzer
    {
        UseTypeIdentifierAnalyzer<TTypeIdentifierAnalyzer>(builder);
        return builder;
    }

    #endregion TypeIdentifierAnalyzer

    #endregion Public 方法
}
