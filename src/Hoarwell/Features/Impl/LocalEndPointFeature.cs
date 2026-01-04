using System.Net;

namespace Hoarwell.Features;

/// <summary>
/// 本地端点特征
/// </summary>
public sealed class LocalEndPointFeature(EndPoint? endPoint) : ILocalEndPointFeature
{
    #region Public 属性

    /// <inheritdoc/>
    public EndPoint? EndPoint { get; } = endPoint;

    #endregion Public 属性
}
