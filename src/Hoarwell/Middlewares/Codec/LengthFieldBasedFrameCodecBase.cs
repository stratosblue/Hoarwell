using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Hoarwell.Middlewares.Codec;

/// <summary>
/// 基于长度帧头的帧编码器基类
/// </summary>
public abstract class LengthFieldBasedFrameCodecBase
{
    #region Public 属性

    /// <summary>
    /// 帧头大小
    /// </summary>
    public int FrameHeaderSize { get; }

    /// <summary>
    /// 最大帧大小
    /// </summary>
    public long MaxFrameLength { get; }

    /// <summary>
    /// 长度是否应包含帧头大小
    /// </summary>
    public bool ShouldLengthIncludeFrameHeaderSize { get; }

    /// <summary>
    /// 是否应该反转端序
    /// </summary>
    public bool ShouldReverseEndianness { get; }

    /// <summary>
    /// 是否使用小端序
    /// </summary>
    public bool UseLittleEndian { get; }

    #endregion Public 属性

    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="LengthFieldBasedFrameCodecBase"/>
    /// </summary>
    /// <param name="frameHeaderSize">帧头大小</param>
    /// <param name="shouldLengthIncludeFrameHeaderSize">长度是否应包含帧头大小</param>
    /// <param name="useLittleEndian">是否使用小端序</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public LengthFieldBasedFrameCodecBase(int frameHeaderSize, bool shouldLengthIncludeFrameHeaderSize = false, bool useLittleEndian = true)
    {
        if (frameHeaderSize < 1
            || frameHeaderSize > sizeof(long))
        {
            throw new ArgumentOutOfRangeException(nameof(frameHeaderSize), $"The length must between 1 and {sizeof(long)}");
        }

        FrameHeaderSize = frameHeaderSize;
        ShouldLengthIncludeFrameHeaderSize = shouldLengthIncludeFrameHeaderSize;
        UseLittleEndian = useLittleEndian;

        ShouldReverseEndianness = BitConverter.IsLittleEndian != UseLittleEndian;

        MaxFrameLength = (long)Math.Pow(2, frameHeaderSize * 8) - 1;
    }

    #endregion Public 构造函数

    #region Protected 方法

    /// <summary>
    /// 读取长度
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    protected long ReadLength(ReadOnlySequence<byte> buffer)
    {
        //SEE https://github.com/dotnet/runtime/blob/0b12d37843e7165fb4c8b794186f19ef43af6c73/src/libraries/System.Memory/src/System/Buffers/SequenceReaderExtensions.Binary.cs

        long length;

        Span<byte> tempSpan = stackalloc byte[sizeof(long)];
        //SEE https://github.com/dotnet/runtime/discussions/74860
        tempSpan.Clear();

        if (buffer.FirstSpan.Length >= FrameHeaderSize)
        {
            buffer.FirstSpan.Slice(0, FrameHeaderSize).CopyTo(tempSpan);
        }
        else
        {
            buffer.Slice(0, FrameHeaderSize).CopyTo(tempSpan);
        }

        length = Unsafe.ReadUnaligned<long>(ref MemoryMarshal.GetReference(tempSpan));

        length = ShouldReverseEndianness
                 ? BinaryPrimitives.ReverseEndianness(length)
                 : length;

        if (!ShouldLengthIncludeFrameHeaderSize)
        {
            length += FrameHeaderSize;
        }

        ThrowIfLengthInvalid(length);

        return length;
    }

    /// <summary>
    /// 长度无效时抛出异常
    /// </summary>
    /// <param name="length"></param>
    /// <exception cref="InvalidFrameLengthException"></exception>
    protected void ThrowIfLengthInvalid(long length)
    {
        if (length < 0)
        {
            throw new InvalidFrameLengthException(length);
        }
        else if (length > MaxFrameLength)
        {
            throw new InvalidFrameLengthException($"The maximum allowable frame length is {MaxFrameLength}", length);
        }
    }

    /// <summary>
    /// 写入长度
    /// </summary>
    /// <param name="length"></param>
    /// <param name="destination"></param>
    protected void WriteLength(long length, Span<byte> destination)
    {
        ThrowIfLengthInvalid(length);

        Span<byte> tempData = stackalloc byte[sizeof(ulong)];
        BinaryPrimitives.WriteInt64LittleEndian(tempData, length);
        tempData = tempData.Slice(0, FrameHeaderSize);
        if (ShouldReverseEndianness)   //这个逻辑需要验证
        {
            tempData.Reverse();
        }
        tempData.CopyTo(destination);
    }

    #endregion Protected 方法
}
