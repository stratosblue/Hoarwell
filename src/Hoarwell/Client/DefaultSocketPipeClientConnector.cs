using System.IO.Pipelines;
using System.Net;
using Hoarwell.Enhancement.IO;
using Hoarwell.Features;
using Hoarwell.Options;

namespace Hoarwell.Client;

internal class DefaultSocketPipeClientConnector(EndPoint endPoint, SocketCreateOptions options)
    : SingletonSocketBaseDuplexPipeConnector<PipeReader, PipeWriter>(endPoint, options)
{
    #region Protected 方法

    protected override async Task<IDuplexPipeContext<PipeReader, PipeWriter>> InternalConnectAsync(CancellationToken cancellationToken)
    {
        var socket = await ConnectSocketAsync(cancellationToken).ConfigureAwait(false);

        CancellationTokenSource? cts = null;

        try
        {
            var readStream = new ReadOnlySocketStream(socket, false);
            var writeStream = new WriteOnlySocketStream(socket, false);
            var pipeReader = PipeReader.Create(readStream);
            var pipeWriter = PipeWriter.Create(writeStream);

            cts = CancellationTokenSource.CreateLinkedTokenSource(readStream.AvailableCancellationToken, writeStream.AvailableCancellationToken);

            var context = new SocketConnectionContext<PipeReader, PipeWriter>(socket: socket,
                                                                              inputter: pipeReader,
                                                                              outputter: pipeWriter,
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

    #endregion Protected 方法
}
