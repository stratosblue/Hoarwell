using System.Net;
using Hoarwell.Options;

namespace Hoarwell.Client;

internal class DefaultSocketStreamClientConnector(EndPoint endPoint, SocketCreateOptions options)
    : SingletonSocketBaseDuplexPipeConnector<Stream, Stream>(endPoint, options)
{
    #region Public 方法

    protected override async Task<IDuplexPipeContext<Stream, Stream>> InternalConnectAsync(CancellationToken cancellationToken)
    {
        var socket = await ConnectSocketAsync(cancellationToken).ConfigureAwait(false);

        return SocketDuplexPipeContextHelper.CreateDefaultStreamContext(socket);
    }

    #endregion Public 方法
}
