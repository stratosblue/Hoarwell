using System.ComponentModel;
using System.Net.Sockets;
using Hoarwell;
using Hoarwell.Build;
using Hoarwell.Options;
using Hoarwell.Transport;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Hoarwell 默认 socket 传输服务端拓展
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class DefaultSocketTransportServerDIExtensions
{
    #region Public 方法

    /// <summary>
    /// 使用默认的基于 <see cref="Socket"/> 的传输服务端
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="builder"></param>
    /// <param name="endpointSetupAction"></param>
    /// <param name="socketSetupAction"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public static HoarwellBuilder<TContext, Stream, Stream> UseDefaultSocketTransportServer<TContext>(this HoarwellBuilder<TContext, Stream, Stream> builder,
                                                                                                      Action<HoarwellEndPointOptions> endpointSetupAction,
                                                                                                      Action<SocketCreateOptions>? socketSetupAction = null,
                                                                                                      ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TContext : IHoarwellContext
    {
        ArgumentNullExceptionHelper.ThrowIfNull(builder);

        var serviceDescriptor = ServiceDescriptor.DescribeKeyed(typeof(IDuplexPipeConnectorFactory<Stream, Stream>), builder.ApplicationName, typeof(SocketServerConnectorFactory), lifetime);

        UseDefaultSocketTransportCore(builder, serviceDescriptor, endpointSetupAction, socketSetupAction);

        return builder;
    }

    #endregion Public 方法

    #region Private 方法

    private static void UseDefaultSocketTransportCore(HoarwellBuilder builder,
                                                      ServiceDescriptor connectorFactoryServiceDescriptor,
                                                      Action<HoarwellEndPointOptions> endpointSetupAction,
                                                      Action<SocketCreateOptions>? socketSetupAction = null)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(endpointSetupAction);

        var services = builder.Services;

        services.TryAdd(connectorFactoryServiceDescriptor);

        services.AddOptions<HoarwellEndPointOptions>(builder.ApplicationName).Configure(endpointSetupAction);

        if (socketSetupAction is not null)
        {
            services.AddOptions<SocketCreateOptions>(builder.ApplicationName).Configure(socketSetupAction);
        }
    }

    #endregion Private 方法
}
