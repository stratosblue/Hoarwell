using System.Net;
using System.Net.Sockets;
using Hoarwell.Options;

namespace Hoarwell.Transport;

/// <summary>
/// 基于 <see cref="Socket"/> 的 <inheritdoc cref="SingleConnectionDuplexPipeConnector{TInputter, TOutputter}"/>
/// </summary>
/// <typeparam name="TInputter"></typeparam>
/// <typeparam name="TOutputter"></typeparam>
/// <param name="remoteEndPoint"></param>
/// <param name="options"></param>
public abstract class SingletonSocketBaseDuplexPipeConnector<TInputter, TOutputter>(EndPoint remoteEndPoint, SocketCreateOptions options)
    : SingleConnectionDuplexPipeConnector<TInputter, TOutputter>
{
    #region Protected 字段

    /// <summary>
    /// <see cref="Socket"/> 创建工厂
    /// </summary>
    protected readonly SocketCreateDelegate SocketCreateFactory = options.SocketCreateFactory ?? SocketCreateOptions.DefaultSocketCreateFactory;

    #endregion Protected 字段

    #region Public 属性

    /// <summary>
    /// 远程 <inheritdoc cref="EndPoint"/>
    /// </summary>
    public EndPoint RemoteEndPoint { get; } = remoteEndPoint;

    #endregion Public 属性

    #region Protected 方法

    /// <summary>
    /// 连接 <see cref="Socket"/>
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task<Socket> ConnectSocketAsync(CancellationToken cancellationToken = default)
    {
        var socket = SocketCreateFactory(RemoteEndPoint);
        await socket.ConnectAsync(RemoteEndPoint).ConfigureAwait(false);
        return socket;
    }

    #endregion Protected 方法
}
