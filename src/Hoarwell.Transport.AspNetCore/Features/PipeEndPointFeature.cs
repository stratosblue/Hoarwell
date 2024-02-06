using System.Net;
using Hoarwell.Features;

namespace Hoarwell.Transport.AspNetCore.Features;

internal sealed class PipeEndPointFeature : IPipeEndPointFeature
{
    #region Public 属性

    public EndPoint? LocalEndPoint { get; }

    public EndPoint? RemoteEndPoint { get; }

    #endregion Public 属性

    #region Public 构造函数

    public PipeEndPointFeature(EndPoint? localEndPoint, EndPoint? remoteEndPoint)
    {
        LocalEndPoint = localEndPoint;
        RemoteEndPoint = remoteEndPoint;
    }

    #endregion Public 构造函数
}
