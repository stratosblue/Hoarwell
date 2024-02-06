namespace Hoarwell.Options;

/// <summary>
/// Hoarwell 选项
/// </summary>
public class HoarwellOptions
{
    #region Public 属性

    /// <summary>
    /// 在处理消息出现异常时关闭管道
    /// </summary>
    public bool ClosePipeOnMessageHandleException { get; set; } = true;

    /// <summary>
    /// 异步处理消息，不阻塞数据读取
    /// </summary>
    public bool HandleMessageAsynchronously { get; set; } = true;

    #endregion Public 属性
}
