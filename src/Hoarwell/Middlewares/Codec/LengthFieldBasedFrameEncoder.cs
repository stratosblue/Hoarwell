using Hoarwell.ExecutionPipeline;

namespace Hoarwell.Middlewares.Codec;

/// <summary>
/// 基于长度帧头的数据帧编码器
/// </summary>
/// <typeparam name="TContext"></typeparam>
public class LengthFieldBasedFrameEncoder<TContext>
    : LengthFieldBasedFrameCodecBase, IPipelineMiddleware<TContext, OutboundMetadata, OutboundMetadata>
    where TContext : IHoarwellContext
{
    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="LengthFieldBasedFrameEncoder{TContext}"/>
    /// </summary>
    /// <param name="frameHeaderSize">帧头大小</param>
    /// <param name="shouldLengthIncludeFrameHeaderSize">长度是否包含帧头大小</param>
    /// <param name="useLittleEndian">是否使用小端序</param>
    public LengthFieldBasedFrameEncoder(int frameHeaderSize, bool shouldLengthIncludeFrameHeaderSize = false, bool useLittleEndian = true)
        : base(frameHeaderSize, shouldLengthIncludeFrameHeaderSize, useLittleEndian)
    {
    }

    #endregion Public 构造函数

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
