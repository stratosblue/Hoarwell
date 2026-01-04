using System.Net;

namespace Hoarwell.Features;

/// <summary>
/// 本地多端点特征
/// </summary>
public sealed class LocalEndPointsFeature(IEnumerable<EndPoint> endPoints) : ILocalEndPointsFeature
{
    #region Public 属性

    /// <inheritdoc/>
    public IEnumerable<EndPoint> EndPoints { get; } = endPoints.ToList().AsReadOnly();

    #endregion Public 属性
}
