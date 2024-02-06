namespace Hoarwell;

/// <summary>
/// Hoarwell 异常
/// </summary>
public class HoarwellException : Exception
{
    #region Public 构造函数

    /// <inheritdoc cref="HoarwellException"/>
    public HoarwellException()
    {
    }

    /// <inheritdoc cref="HoarwellException"/>
    public HoarwellException(string? message) : base(message)
    {
    }

    /// <inheritdoc cref="HoarwellException"/>
    public HoarwellException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    #endregion Public 构造函数
}
