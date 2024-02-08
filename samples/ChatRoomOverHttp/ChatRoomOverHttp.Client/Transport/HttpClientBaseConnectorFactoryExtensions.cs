using ChatRoomOverHttp.Server.Transport;
using Hoarwell;
using Hoarwell.Build;
using Hoarwell.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

internal static class HttpClientBaseConnectorFactoryExtensions
{
    #region Public 方法

    public static HoarwellBuilder<TContext, Stream, Stream> UseHttpBaseTransportClient<TContext>(this HoarwellBuilder<TContext, Stream, Stream> builder, Action<HoarwellEndPointOptions> setupAction)
        where TContext : IHoarwellContext
    {
        var serviceDescriptor = ServiceDescriptor.DescribeKeyed(typeof(IDuplexPipeConnectorFactory<Stream, Stream>), builder.ApplicationName, typeof(HttpClientBaseConnectorFactory), ServiceLifetime.Singleton);

        builder.Services.TryAdd(serviceDescriptor);

        builder.Services.AddOptions<HoarwellEndPointOptions>(builder.ApplicationName).Configure(setupAction);

        return builder;
    }

    #endregion Public 方法
}
