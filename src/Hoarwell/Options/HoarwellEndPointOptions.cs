using System.Net;

namespace Hoarwell.Options;

/// <summary>
/// Hoarwell 终结点选项
/// </summary>
public class HoarwellEndPointOptions
{
    #region Public 属性

    /// <summary>
    /// 终结点列表
    /// </summary>
    public List<EndPoint> EndPoints { get; } = [];

    #endregion Public 属性
}
