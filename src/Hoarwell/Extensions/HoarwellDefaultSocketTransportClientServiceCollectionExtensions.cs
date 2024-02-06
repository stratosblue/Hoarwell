using System.ComponentModel;
using System.IO.Pipelines;
using System.Net.Sockets;
using Hoarwell;
using Hoarwell.Build;
using Hoarwell.Client;
using Hoarwell.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Hoarwell 默认 socket 传输客户端拓展
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class HoarwellDefaultSocketTransportClientServiceCollectionExtensions

{
    #region Public 方法

    /// <summary>
    /// 使用默认的基于 <see cref="Socket"/> 的传输客户端
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="builder"></param>
    /// <param name="setupAction"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public static HoarwellBuilder<TContext, PipeReader, PipeWriter> UseDefaultSocketTransportClient<TContext>(this HoarwellBuilder<TContext, PipeReader, PipeWriter> builder,
                                                                                                              Action<HoarwellEndPointOptions> setupAction,
                                                                                                              ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TContext : IHoarwellContext
    {
        ArgumentNullExceptionHelper.ThrowIfNull(builder);
        ArgumentNullExceptionHelper.ThrowIfNull(setupAction);

        var services = builder.Services;

        services.TryAdd(ServiceDescriptor.DescribeKeyed(typeof(IDuplexPipeConnectorFactory<PipeReader, PipeWriter>), builder.ApplicationName, typeof(DefaultSocketPipeClientConnectorFactory), lifetime));

        services.AddOptions<HoarwellEndPointOptions>(builder.ApplicationName).Configure(setupAction);

        return builder;
    }

    /// <summary>
    /// 使用默认的基于 <see cref="Socket"/> 的传输客户端
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="builder"></param>
    /// <param name="setupAction"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public static HoarwellBuilder<TContext, Stream, Stream> UseDefaultSocketTransportClient<TContext>(this HoarwellBuilder<TContext, Stream, Stream> builder,
                                                                                                      Action<HoarwellEndPointOptions> setupAction,
                                                                                                      ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TContext : IHoarwellContext
    {
        ArgumentNullExceptionHelper.ThrowIfNull(builder);
        ArgumentNullExceptionHelper.ThrowIfNull(setupAction);

        var services = builder.Services;

        services.TryAdd(ServiceDescriptor.DescribeKeyed(typeof(IDuplexPipeConnectorFactory<Stream, Stream>), builder.ApplicationName, typeof(DefaultSocketStreamClientConnectorFactory), lifetime));

        services.AddOptions<HoarwellEndPointOptions>(builder.ApplicationName).Configure(setupAction);

        return builder;
    }

    #endregion Public 方法
}
