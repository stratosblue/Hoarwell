using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Hoarwell.ExecutionPipeline;

namespace Hoarwell.Middlewares.Codec;

/// <summary>
/// 基于长度帧头（uint32）的数据帧解码器
/// </summary>
/// <typeparam name="TContext"></typeparam>
public class UInt32LengthFieldBasedFrameDecoder<TContext>
    : IPipelineMiddleware<TContext, PipeReader, ReadOnlySequence<byte>>
    where TContext : IHoarwellContext
{
    #region Singleton

    /// <summary>
    /// UInt32LengthFieldBasedFrameDecoder静态实例
    /// </summary>
    public static UInt32LengthFieldBasedFrameDecoder<TContext> Shared { get; } = new();

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
