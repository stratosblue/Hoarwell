using System.Net;
using System.Net.Sockets;

namespace Hoarwell.Options;

/// <summary>
/// <see cref="Socket"/> 创建选项
/// </summary>
public class SocketCreateOptions
{
    #region Public 属性

    /// <summary>
    /// 默认的 <see cref="Socket"/> 创建委托
    /// </summary>
    public static SocketCreateDelegate DefaultSocketCreateFactory { get; } = CreateDefaultSocket;

    /// <summary>
    /// <see cref="Socket"/> 创建委托
    /// </summary>
    public SocketCreateDelegate? SocketCreateFactory { get; set; }

    #endregion Public 属性

    #region Public 方法

    /// <summary>
    /// 使用 <paramref name="endPoint"/> 创建默认的 <see cref="Socket"/>
    /// </summary>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    public static Socket CreateDefaultSocket(EndPoint endPoint)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(endPoint);

        return new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    }

    #endregion Public 方法
}
