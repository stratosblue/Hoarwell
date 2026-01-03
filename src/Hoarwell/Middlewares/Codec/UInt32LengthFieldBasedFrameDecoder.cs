using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Hoarwell.ExecutionPipeline;

namespace Hoarwell.Middlewares.Codec;

/// <summary>
/// 基于长度帧头（uint32）的数据帧编码器选项
/// </summary>
public record class UInt32LengthFieldBasedFrameCodecOptions
{
    #region Public 字段

    /// <summary>
    /// 默认最大帧大小
    /// </summary>
    public const uint DefaultMaxFrameSize = 30_000_000;

    #endregion Public 字段

    #region Public 属性

    /// <summary>
    /// 最大帧大小
    /// </summary>
    public uint? MaxFrameSize { get; set; } = DefaultMaxFrameSize;

    #endregion Public 属性
}

/// <summary>
/// 基于长度帧头（uint32）的数据帧解码器
/// </summary>
/// <param name="options">选项</param>
/// <typeparam name="TContext"></typeparam>
public class UInt32LengthFieldBasedFrameDecoder<TContext>(UInt32LengthFieldBasedFrameCodecOptions options)
    : IPipelineMiddleware<TContext, PipeReader, ReadOnlySequence<byte>>
    where TContext : IHoarwellContext
{
    #region Private 字段

    private readonly uint _maxFrameSize = options.MaxFrameSize ?? uint.MaxValue;

    #endregion Private 字段

    #region Singleton

    /// <summary>
    /// <see cref="UInt32LengthFieldBasedFrameDecoder{TContext}"/> 静态实例
    /// </summary>
    public static UInt32LengthFieldBasedFrameDecoder<TContext> Shared { get; } = new(new());

    #endregion Singleton

    #region Public 字段

    /// <summary>
    /// 帧头大小
    /// </summary>
    public const int FrameHeaderSize = sizeof(uint);

    #endregion Public 字段

    #region Public 方法

    /// <inheritdoc/>
    public async Task InvokeAsync(TContext context, PipeReader input, PipelineInvokeDelegate<TContext, ReadOnlySequence<byte>> next)
    {
        var cancellationToken = context.ExecutionAborted;
        var pipeReader = input;

        var totalLength = 0u;
        var totalLengthNotComputedYet = true;

        while (!cancellationToken.IsCancellationRequested)
        {
            var readResult = await pipeReader.ReadAsync(cancellationToken).ConfigureAwait(false);

            var buffer = readResult.Buffer;

            if (buffer.IsEmpty
                && readResult.IsCompleted)
            {
                break;
            }

            if (totalLengthNotComputedYet)
            {
                if (buffer.Length < FrameHeaderSize)
                {
                    pipeReader.AdvanceTo(buffer.Start, buffer.End);
                    continue;
                }

                totalLength = ReadLength(buffer.Slice(0, FrameHeaderSize)) + FrameHeaderSize;

                if (totalLength == FrameHeaderSize)   //HACK 允许长度为0的帧是否不科学
                {
                    pipeReader.AdvanceTo(buffer.GetPosition(FrameHeaderSize));
                    continue;
                }

                if (totalLength > _maxFrameSize)
                {
                    throw new InvalidDataException($"Invalid frame length \"{totalLength}\". Maximum available size is \"{_maxFrameSize}\".");
                }

                totalLengthNotComputedYet = false;
            }

            if (buffer.Length < totalLength)
            {
                pipeReader.AdvanceTo(buffer.Start, buffer.End);
                continue;
            }

            await next(context, buffer.Slice(FrameHeaderSize, totalLength - FrameHeaderSize)).ConfigureAwait(false);

            pipeReader.AdvanceTo(buffer.GetPosition(totalLength));

            totalLengthNotComputedYet = true;
        }

        static uint ReadLength(ReadOnlySequence<byte> buffer)
        {
            //SEE https://github.com/dotnet/runtime/blob/0b12d37843e7165fb4c8b794186f19ef43af6c73/src/libraries/System.Memory/src/System/Buffers/SequenceReaderExtensions.Binary.cs

            if (buffer.FirstSpan.Length >= FrameHeaderSize)
            {
                return Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference(buffer.FirstSpan));
            }

            Span<byte> tempSpan = stackalloc byte[FrameHeaderSize];
            buffer.Slice(0, FrameHeaderSize).CopyTo(tempSpan);

            if (!BitConverter.IsLittleEndian)   //这个逻辑需要验证
            {
                tempSpan.Reverse();
            }

            return Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference(tempSpan));
        }
    }

    #endregion Public 方法
}
