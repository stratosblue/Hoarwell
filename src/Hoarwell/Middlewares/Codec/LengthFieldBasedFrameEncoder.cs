using Hoarwell.ExecutionPipeline;

namespace Hoarwell.Middlewares.Codec;

/// <summary>
/// 基于长度帧头的数据帧编码器
/// </summary>
/// <typeparam name="TContext"></typeparam>
public class LengthFieldBasedFrameEncoder<TContext>(LengthFieldBasedFrameCodecOptions options)
    : LengthFieldBasedFrameCodecBase(options)
    , IPipelineMiddleware<TContext, OutboundMetadata, OutboundMetadata>
    where TContext : IHoarwellContext
{
    #region Public 方法

    /// <inheritdoc/>
    public async Task InvokeAsync(TContext context, OutboundMetadata input, PipelineInvokeDelegate<TContext, OutboundMetadata> next)
    {
        var bufferWriter = input.BufferWriter;
        bufferWriter.Advance(FrameHeaderSize);

        var anchor = bufferWriter.WrittenCount;

        await next(context, input).ConfigureAwait(false);

        var length = ShouldLengthIncludeFrameHeaderSize
                     ? Convert.ToInt64(bufferWriter.WrittenCount - anchor + FrameHeaderSize)
                     : Convert.ToInt64(bufferWriter.WrittenCount - anchor);

        WriteLength(length, bufferWriter.GetSpan(anchor - FrameHeaderSize, FrameHeaderSize));
    }

    #endregion Public 方法
}
