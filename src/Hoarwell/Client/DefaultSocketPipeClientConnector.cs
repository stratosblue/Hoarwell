using System.IO.Pipelines;
using System.Net;
using Hoarwell.Options;
using Hoarwell.Transport;

namespace Hoarwell.Client;

internal class DefaultSocketPipeClientConnector(EndPoint endPoint, SocketCreateOptions options)
    : SingletonSocketBaseDuplexPipeConnector<PipeReader, PipeWriter>(endPoint, options)
{
    #region Protected 方法

    protected override async Task<IDuplexPipeContext<PipeReader, PipeWriter>> InternalConnectAsync(CancellationToken cancellationToken)
    {
        var socket = await ConnectSocketAsync(cancellationToken).ConfigureAwait(false);

        return SocketDuplexPipeContextHelper.CreateDefaultPipeContext(socket);
    }

    #endregion Protected 方法
}
