using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace Hoarwell;

/// <summary>
/// 类型标识符分析器
/// </summary>
public interface ITypeIdentifierAnalyzer
{
    #region Public 属性

    /// <summary>
    /// 类型标识符大小（byte 长度）
    /// </summary>
    public uint TypeIdentifierSize { get; }

    #endregion Public 属性

    #region Public 方法

    /// <summary>
    /// 尝试获取类型 <paramref name="type"/> 的标识符并写入 <paramref name="destination"/>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public bool TryGetIdentifier(in Type type, Span<byte> destination);

    /// <summary>
    /// 尝试从 <paramref name="input"/> 中获取类型的标识符并写入 <paramref name="destination"/>
    /// </summary>
    /// <param name="input"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public bool TryGetIdentifier(in ReadOnlySequence<byte> input, Span<byte> destination);

    /// <summary>
    /// 尝试获取类型 <paramref name="type"/> 的标识符并以 <paramref name="identifier"/> 返回
    /// </summary>
    /// <param name="type"></param>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public bool TryGetIdentifier(in Type type, out ReadOnlyMemory<byte> identifier);

    /// <summary>
    /// 尝试从 <paramref name="input"/> 中获取类型的标识符并以 <paramref name="identifier"/> 返回
    /// </summary>
    /// <param name="input"></param>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public bool TryGetIdentifier(in ReadOnlySequence<byte> input, out ReadOnlyMemory<byte> identifier);

    /// <summary>
    /// 尝试使用类型标识 <paramref name="identifier"/> 获取其类型 <paramref name="type"/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool TryGetType(in ReadOnlySpan<byte> identifier, [NotNullWhen(true)] out Type? type);

    #endregion Public 方法
}
