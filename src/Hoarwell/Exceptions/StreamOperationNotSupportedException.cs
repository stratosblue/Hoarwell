namespace Hoarwell;

/// <summary>
/// 不支持的流操作异常
/// </summary>
public class StreamOperationNotSupportedException : HoarwellException
{
    #region Public 构造函数

    /// <inheritdoc cref="StreamOperationNotSupportedException"/>
    public StreamOperationNotSupportedException(string? message) : base(message)
    {
    }

    #endregion Public 构造函数
}
