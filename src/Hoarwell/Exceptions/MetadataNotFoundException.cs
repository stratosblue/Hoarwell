namespace Hoarwell;

/// <summary>
/// 元数据未找到异常
/// </summary>
public class MetadataNotFoundException : HoarwellException
{
    #region Public 构造函数

    /// <inheritdoc cref="MetadataNotFoundException"/>
    public MetadataNotFoundException(string? message) : base(message)
    {
    }

    /// <inheritdoc cref="MetadataNotFoundException"/>
    public MetadataNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    #endregion Public 构造函数
}
