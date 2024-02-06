using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Hoarwell.Enhancement.Buffers;

/// <summary>
/// 小数据对齐读取工具
/// </summary>
public static class SmallDataAlignReadUtil
{
    #region Public 方法

    /// <summary>
    /// 对齐 <paramref name="bytes"/> 为 int32 读取
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int UnsafeReadAsInt32(ReadOnlySpan<byte> bytes)
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        buffer.Clear();
        bytes.CopyTo(buffer);
        return Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(buffer));
    }

    /// <summary>
    /// 对齐 <paramref name="sequence"/> 为 int32 读取
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int UnsafeReadAsInt32(ReadOnlySequence<byte> sequence)
    {
        if (sequence.FirstSpan.Length >= sizeof(int))
        {
            return Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(sequence.FirstSpan));
        }
        return InternalUnsafeReadAsInt32(sequence);
    }

    #endregion Public 方法

    #region Private 方法

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int InternalUnsafeReadAsInt32(ReadOnlySequence<byte> sequence)
    {
        if (sequence.IsSingleSegment)
        {
            return UnsafeReadAsInt32(sequence.FirstSpan);
        }

        var sequenceReader = new SequenceReader<byte>(sequence);

        sequenceReader.TryReadLittleEndian(out int value);
        return value;
    }

    #endregion Private 方法
}
