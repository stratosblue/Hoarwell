using System.Buffers;
using System.Buffers.Binary;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hoarwell.Enhancement.Buffers;

/// <summary>
/// <see cref="IBufferWriter{T}"/> 写入拓展方法集合
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ByteBufferWriterExtensions
{
    #region byte

    /// <summary>
    /// 将 <paramref name="value"/> 写入 <paramref name="bufferWriter"/>
    /// </summary>
    /// <param name="bufferWriter"></param>
    /// <param name="value"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this IBufferWriter<byte> bufferWriter, in byte value)
    {
        var span = bufferWriter.GetSpan(sizeof(byte));
        span[0] = value;
        bufferWriter.Advance(sizeof(byte));
    }

    #endregion byte

    #region LittleEndian

    #region int16

    /// <summary>
    /// 将 <paramref name="value"/> 以 LittleEndian 写入 <paramref name="bufferWriter"/>
    /// </summary>
    /// <param name="bufferWriter"></param>
    /// <param name="value"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLittleEndian(this IBufferWriter<byte> bufferWriter, in short value)
    {
        var span = bufferWriter.GetSpan(sizeof(short));
        BinaryPrimitives.WriteInt16LittleEndian(span, value);
        bufferWriter.Advance(sizeof(short));
    }

    #endregion int16

    #region uint16

    /// <inheritdoc cref="WriteLittleEndian(IBufferWriter{byte}, in short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLittleEndian(this IBufferWriter<byte> bufferWriter, in ushort value)
    {
        var span = bufferWriter.GetSpan(sizeof(ushort));
        BinaryPrimitives.WriteUInt16LittleEndian(span, value);
        bufferWriter.Advance(sizeof(ushort));
    }

    #endregion uint16

    #region int32

    /// <inheritdoc cref="WriteLittleEndian(IBufferWriter{byte}, in short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLittleEndian(this IBufferWriter<byte> bufferWriter, in int value)
    {
        var span = bufferWriter.GetSpan(sizeof(int));
        BinaryPrimitives.WriteInt32LittleEndian(span, value);
        bufferWriter.Advance(sizeof(int));
    }

    #endregion int32

    #region uint32

    /// <inheritdoc cref="WriteLittleEndian(IBufferWriter{byte}, in short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLittleEndian(this IBufferWriter<byte> bufferWriter, in uint value)
    {
        var span = bufferWriter.GetSpan(sizeof(uint));
        BinaryPrimitives.WriteUInt32LittleEndian(span, value);
        bufferWriter.Advance(sizeof(uint));
    }

    #endregion uint32

    #region int64

    /// <inheritdoc cref="WriteLittleEndian(IBufferWriter{byte}, in short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLittleEndian(this IBufferWriter<byte> bufferWriter, in long value)
    {
        var span = bufferWriter.GetSpan(sizeof(long));
        BinaryPrimitives.WriteInt64LittleEndian(span, value);
        bufferWriter.Advance(sizeof(long));
    }

    #endregion int64

    #region uint64

    /// <inheritdoc cref="WriteLittleEndian(IBufferWriter{byte}, in short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLittleEndian(this IBufferWriter<byte> bufferWriter, in ulong value)
    {
        var span = bufferWriter.GetSpan(sizeof(ulong));
        BinaryPrimitives.WriteUInt64LittleEndian(span, value);
        bufferWriter.Advance(sizeof(ulong));
    }

    #endregion uint64

    #endregion LittleEndian
}
