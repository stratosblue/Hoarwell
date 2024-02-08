using ChatRoomOverHttp.Server.Transport;
using Hoarwell;
using Hoarwell.Build;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

internal static class HttpBaseConnectorFactoryExtensions
{
    #region Public 方法

    public static HoarwellBuilder<TContext, Stream, Stream> UseHttpBaseTransportServer<TContext>(this HoarwellBuilder<TContext, Stream, Stream> builder)
        where TContext : IHoarwellContext
    {
        var serviceDescriptor = ServiceDescriptor.DescribeKeyed(typeof(IDuplexPipeConnectorFactory<Stream, Stream>), builder.ApplicationName, typeof(HttpBaseConnectorFactory), ServiceLifetime.Singleton);

        builder.Services.TryAdd(serviceDescriptor);
        builder.Services.TryAddSingleton<HttpBasePipeConnector>();

        return builder;
    }

    #endregion Public 方法
}
