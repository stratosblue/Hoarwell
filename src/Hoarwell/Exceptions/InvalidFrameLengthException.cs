namespace Hoarwell;

/// <summary>
/// 无效帧长度异常
/// </summary>
public class InvalidFrameLengthException : HoarwellException
{
    #region Public 属性

    /// <summary>
    /// 无效的长度值
    /// </summary>
    public long Value { get; }

    #endregion Public 属性

    #region Public 构造函数

    /// <inheritdoc cref="InvalidFrameLengthException"/>
    public InvalidFrameLengthException(long value) : this(null, value)
    {
    }

    /// <inheritdoc cref="InvalidFrameLengthException"/>
    public InvalidFrameLengthException(string? message, long value) : base(message ?? $"The frame length \"{value}\" is invalid")
    {
        Value = value;
    }

    #endregion Public 构造函数
}
