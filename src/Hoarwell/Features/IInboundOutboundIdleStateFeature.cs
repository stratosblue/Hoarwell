namespace Hoarwell.Features;

/// <summary>
/// 入站出站空闲状态特征
/// </summary>
public interface IInboundOutboundIdleStateFeature
{
    #region Public 事件

    /// <summary>
    /// 触发空闲状态
    /// </summary>
    public event IdleStateTriggeredDelegate OnIdleStateTriggered;

    #endregion Public 事件

    #region Public 属性

    /// <summary>
    /// 最后入站时间（UTC）
    /// </summary>
    public DateTime? LastInbound { get; }

    /// <summary>
    /// 最后出站时间（UTC）
    /// </summary>
    public DateTime? LastOutbound { get; }

    #endregion Public 属性

    #region Public 方法

    /// <summary>
    /// 更新输入时间
    /// </summary>
    public void UpdateInboundTime();

    /// <summary>
    /// 更新输出时间
    /// </summary>
    public void UpdateOutboundTime();

    #endregion Public 方法
}
