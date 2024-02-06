#if NET7_0_OR_GREATER

namespace Hoarwell;

/// <summary>
/// 类型标识符提供者静态接口
/// </summary>
public interface ITypeIdentifierProvider
{
    #region Public 属性

    /// <summary>
    /// 当前类型的类型标识
    /// </summary>
    public static abstract ReadOnlySpan<byte> TypeIdentifier { get; }

    #endregion Public 属性
}

#endif
