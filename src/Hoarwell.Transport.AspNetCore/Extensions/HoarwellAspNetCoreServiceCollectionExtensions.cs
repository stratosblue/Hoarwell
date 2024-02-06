using System.ComponentModel;
using System.IO.Pipelines;
using Hoarwell;
using Hoarwell.Transport.AspNetCore;
using Hoarwell.Build;
using Hoarwell.Options;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class HoarwellAspNetCoreServiceCollectionExtensions
{
    #region Public 方法

    /// <summary>
    /// 使用 AspNetCore 的 <see cref="SocketTransportFactory"/> 的传输服务端
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="builder"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static HoarwellBuilder<TContext, PipeReader, PipeWriter> UseAspNetCoreSocketTransportServer<TContext>(this HoarwellBuilder<TContext, PipeReader, PipeWriter> builder, Action<HoarwellEndPointOptions> setupAction)
        where TContext : IHoarwellContext
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(setupAction);

        var services = builder.Services;

        services.AddLogging();
        services.AddOptions<SocketTransportOptions>(builder.ApplicationName);
        services.TryAddKeyedSingleton<IConnectionListenerFactory, SocketTransportFactory>(builder.ApplicationName);
        services.TryAddKeyedSingleton<IDuplexPipeConnectorFactory<PipeReader, PipeWriter>, SocketTransportFactoryServerConnectorFactory>(builder.ApplicationName);

        services.AddOptions<HoarwellEndPointOptions>(builder.ApplicationName).Configure(setupAction);

        return builder;
    }

    #endregion Public 方法
}
