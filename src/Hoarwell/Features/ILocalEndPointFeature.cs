using System.Net;

namespace Hoarwell.Features;

/// <summary>
/// 本地端点特征
/// </summary>
public interface ILocalEndPointFeature
{
    #region Public 属性

    /// <summary>
    /// 本地端点
    /// </summary>
    public EndPoint? EndPoint { get; }

    #endregion Public 属性
}

/// <summary>
/// 本地多端点特征
/// </summary>
public interface ILocalEndPointsFeature
{
    #region Public 属性

    /// <summary>
    /// 本地端点列表
    /// </summary>
    public IEnumerable<EndPoint> EndPoints { get; }

    #endregion Public 属性
}
