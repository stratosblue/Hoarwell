using System.Net.Sockets;
using Hoarwell.Features;

namespace Hoarwell.Transport;

/// <summary>
/// <see cref="Socket"/> 监听连接器
/// </summary>
public sealed class SocketListenerConnector : IDuplexPipeConnector<Stream, Stream>
{
    #region Private 字段

    private readonly Socket _socket;

    #endregion Private 字段

    #region Public 属性

    /// <inheritdoc/>
    public IFeatureCollection Features { get; }

    #endregion Public 属性

    #region Public 构造函数

    /// <inheritdoc cref="SocketListenerConnector"/>
    public SocketListenerConnector(Socket socket)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(socket, nameof(socket));

        Features = ImmutableFeatureCollection.Builder()
                                             .Add<ILocalEndPointFeature>(new LocalEndPointFeature(socket.LocalEndPoint))
                                             .Build();
        _socket = socket;
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public async ValueTask<IDuplexPipeContext<Stream, Stream>> ConnectAsync(CancellationToken cancellationToken)
    {
#if NET7_0_OR_GREATER

        var socket = await _socket.AcceptAsync(cancellationToken).ConfigureAwait(false);

#else

        var socket = await _socket.AcceptAsync().ConfigureAwait(false);

#endif

        return SocketDuplexPipeContextHelper.CreateDefaultStreamContext(socket);
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        _socket.Dispose();
        return default;
    }

    /// <inheritdoc/>
    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        //TODO timeout optionable
        _socket.Close();
        return default;
    }

    #endregion Public 方法
}
