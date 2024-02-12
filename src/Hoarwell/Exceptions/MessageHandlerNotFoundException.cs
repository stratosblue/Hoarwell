namespace Hoarwell;

/// <summary>
/// 消息对应 <see cref="IEndpointMessageHandler{T}"/> 未找到的异常
/// </summary>
public class MessageHandlerNotFoundException : HoarwellException
{
    #region Public 属性

    /// <summary>
    /// 目标消息
    /// </summary>
    public object? TargetMessage { get; }

    /// <summary>
    /// 目标消息类型
    /// </summary>
    public Type TargetMessageType { get; }

    #endregion Public 属性

    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="MessageHandlerNotFoundException"/>
    /// </summary>
    /// <param name="targetMessage"></param>
    /// <param name="targetMessageType"></param>
    /// <param name="message"></param>
    public MessageHandlerNotFoundException(object? targetMessage, Type targetMessageType, string? message = null) : this(targetMessage, targetMessageType, null, message)
    {
    }

    /// <summary>
    /// <inheritdoc cref="MessageHandlerNotFoundException"/>
    /// </summary>
    /// <param name="targetMessage"></param>
    /// <param name="targetMessageType"></param>
    /// <param name="innerException"></param>
    /// <param name="message"></param>
    public MessageHandlerNotFoundException(object? targetMessage, Type targetMessageType, Exception? innerException, string? message = null) : base(message ?? $"Not found message handler for {targetMessageType}", innerException)
    {
        TargetMessage = targetMessage;
        TargetMessageType = targetMessageType;
    }

    #endregion Public 构造函数
}
