using System.Net;

namespace ChatRoomOverHttp.Client.Transport;

//仅用作功能展示

internal class HttpEndPoint : EndPoint
{
    #region Private 字段

    private readonly string _url;

    #endregion Private 字段

    #region Public 构造函数

    public HttpEndPoint(string url)
    {
        _url = url ?? throw new ArgumentNullException(nameof(url));
    }

    #endregion Public 构造函数

    #region Public 方法

    public override string ToString()
    {
        return _url;
    }

    #endregion Public 方法
}
