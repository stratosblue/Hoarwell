using System.Net;
using Hoarwell.Enhancement.IO;
using Hoarwell.Features;
using Hoarwell.Options;

namespace Hoarwell.Client;

internal class DefaultSocketStreamClientConnector(EndPoint endPoint, SocketCreateOptions options)
    : SingletonSocketBaseDuplexPipeConnector<Stream, Stream>(endPoint, options)
{
    #region Public 方法

    protected override async Task<IDuplexPipeContext<Stream, Stream>> InternalConnectAsync(CancellationToken cancellationToken)
    {
        var socket = await ConnectSocketAsync(cancellationToken).ConfigureAwait(false);

        CancellationTokenSource? cts = null;

        try
        {
            var readStream = new ReadOnlySocketStream(socket, false);
            var writeStream = new WriteOnlySocketStream(socket, false);

            cts = CancellationTokenSource.CreateLinkedTokenSource(readStream.AvailableCancellationToken, writeStream.AvailableCancellationToken);

            var context = new SocketConnectionContext<Stream, Stream>(socket: socket,
                                                                      inputter: readStream,
                                                                      outputter: writeStream,
                                                                      lifetimeFeature: new DelegatingPipeLifetimeFeature(cts.Token, socket.Close),
                                                                      disposeCallback: cts.Dispose);

            context.Features.Set<IPipeEndPointFeature>(new PipeEndPointFeature(socket.LocalEndPoint!, RemoteEndPoint));

            return context;
        }
        catch
        {
            cts.SilenceRelease();
            socket.Close();
            throw;
        }
    }

    #endregion Public 方法
}
