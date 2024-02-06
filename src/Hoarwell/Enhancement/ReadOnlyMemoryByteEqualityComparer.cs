using System.Diagnostics.CodeAnalysis;

namespace Hoarwell.Enhancement;

/// <summary>
/// 针对 <see cref="ReadOnlyMemory{T}"/> byte 的 <see cref="IEqualityComparer{T}"/>
/// </summary>
public class ReadOnlyMemoryByteEqualityComparer : IEqualityComparer<ReadOnlyMemory<byte>>
{
    #region Public 属性

    /// <summary>
    /// 共享实例
    /// </summary>
    public static ReadOnlyMemoryByteEqualityComparer Shared { get; } = new();

    #endregion Public 属性

    #region Private 构造函数

    private ReadOnlyMemoryByteEqualityComparer()
    {
    }

    #endregion Private 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public bool Equals(ReadOnlyMemory<byte> x, ReadOnlyMemory<byte> y) => x.Span.SequenceEqual(y.Span);

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] ReadOnlyMemory<byte> obj)
    {
        var hashCode = new HashCode();
        hashCode.AddBytes(obj.Span);
        return hashCode.ToHashCode();
    }

    #endregion Public 方法
}
