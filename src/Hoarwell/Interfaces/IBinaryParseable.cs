#if NET7_0_OR_GREATER

using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace Hoarwell;

/// <summary>
/// 可从二进制数据解析
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IBinaryParseable<T>
{
    #region Public 方法

    /// <summary>
    /// 尝试从 <paramref name="input"/> 解析结果
    /// </summary>
    /// <param name="input">二进制数据</param>
    /// <param name="result">解析结果</param>
    /// <returns></returns>
    public static abstract bool TryParse(in ReadOnlySequence<byte> input, [MaybeNullWhen(false)] out T result);

    #endregion Public 方法
}

#endif
