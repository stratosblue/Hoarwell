using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Hoarwell.ExecutionPipeline;

namespace Hoarwell.Middlewares.Codec;

/// <summary>
/// 基于长度帧头（uint32）的数据帧编码器
/// </summary>
/// <typeparam name="TContext"></typeparam>
public class UInt32LengthFieldBasedFrameEncoder<TContext>(UInt32LengthFieldBasedFrameCodecOptions options)
    : IPipelineMiddleware<TContext, OutboundMetadata, OutboundMetadata>
    where TContext : IHoarwellContext
{
    #region Private 字段

    private readonly uint _maxFrameSize = options.MaxFrameSize ?? uint.MaxValue;

    #endregion Private 字段

    #region Singleton

    /// <summary>
    /// UInt32LengthFieldBasedFrameEncoder静态实例
    /// </summary>
    public static UInt32LengthFieldBasedFrameEncoder<TContext> Shared { get; } = new(new());

    #endregion Singleton

    #region Public 字段

    /// <summary>
    /// 帧头大小
    /// </summary>
    public const int FrameHeaderSize = sizeof(uint);

    #endregion Public 字段

    #region Public 方法

    /// <inheritdoc/>
    public async Task InvokeAsync(TContext context, OutboundMetadata input, PipelineInvokeDelegate<TContext, OutboundMetadata> next)
    {
        var bufferWriter = input.BufferWriter;
        bufferWriter.Advance(FrameHeaderSize);

        var anchor = bufferWriter.WrittenCount;

        await next(context, input).ConfigureAwait(false);

        var length = (uint)(bufferWriter.WrittenCount - anchor);

        if (length > _maxFrameSize)
        {
            throw new InvalidDataException($"Invalid frame length \"{length}\". Maximum available size is \"{_maxFrameSize}\".");
        }

        Unsafe.WriteUnaligned<uint>(ref MemoryMarshal.GetReference(bufferWriter.GetSpan(anchor - FrameHeaderSize, FrameHeaderSize)), length);

        if (!BitConverter.IsLittleEndian)   //这个逻辑需要验证
        {
            bufferWriter.GetSpan(anchor - FrameHeaderSize, FrameHeaderSize).Reverse();
        }
    }

    #endregion Public 方法
}
