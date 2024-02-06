using System.Buffers;
using System.IO.Pipelines;
using Hoarwell.ExecutionPipeline;

namespace Hoarwell.Middlewares.Codec;

/// <summary>
/// 基于长度帧头的数据帧解码器
/// </summary>
/// <typeparam name="TContext"></typeparam>
public class LengthFieldBasedFrameDecoder<TContext>
    : LengthFieldBasedFrameCodecBase, IPipelineMiddleware<TContext, PipeReader, ReadOnlySequence<byte>>
    where TContext : IHoarwellContext
{
    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="LengthFieldBasedFrameDecoder{TContext}"/>
    /// </summary>
    /// <param name="frameHeaderSize">帧头大小</param>
    /// <param name="shouldLengthIncludeFrameHeaderSize">长度是否包含帧头大小</param>
    /// <param name="useLittleEndian">是否使用小端序</param>
    public LengthFieldBasedFrameDecoder(int frameHeaderSize, bool shouldLengthIncludeFrameHeaderSize = false, bool useLittleEndian = true)
        : base(frameHeaderSize, shouldLengthIncludeFrameHeaderSize, useLittleEndian)
    {
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public async Task InvokeAsync(TContext context, PipeReader input, PipelineInvokeDelegate<TContext, ReadOnlySequence<byte>> next)
    {
        var cancellationToken = context.ExecutionAborted;
        var pipeReader = input;

        var totalLength = 0L;
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

                totalLength = ReadLength(buffer.Slice(0, FrameHeaderSize));

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
    }

    #endregion Public 方法
}
