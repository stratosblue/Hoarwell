namespace Hoarwell.Options.Features;

/// <summary>
/// 入站出站空闲选项
/// </summary>
public class InboundOutboundIdleOptions
{
    #region Public 属性

    /// <summary>
    /// 入站空闲超时
    /// </summary>
    public TimeSpan? InboundIdleTimeout { get; set; }

    /// <summary>
    /// 出站空闲超时
    /// </summary>
    public TimeSpan? OutboundIdleTimeout { get; set; }

    #endregion Public 属性
}
