using System.Net;

namespace Hoarwell.Features;

/// <summary>
/// 管道节点特征
/// </summary>
public interface IPipeEndPointFeature
{
    #region Public 属性

    /// <summary>
    /// 本地终结点
    /// </summary>
    EndPoint? LocalEndPoint { get; }

    /// <summary>
    /// 远程终结点
    /// </summary>
    EndPoint? RemoteEndPoint { get; }

    #endregion Public 属性
}
