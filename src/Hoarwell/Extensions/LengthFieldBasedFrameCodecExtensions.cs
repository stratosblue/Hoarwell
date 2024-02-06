using System.Buffers;
using System.ComponentModel;
using System.IO.Pipelines;
using Hoarwell;
using Hoarwell.Build;
using Hoarwell.Middlewares.Codec;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// LengthFieldBasedFrameCodec 拓展方法集合
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class LengthFieldBasedFrameCodecExtensions
{
    #region Public 方法

    #region UInt32

    /// <summary>
    /// 使用默认的基于 uin32 的帧解码器
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static InboundPipelineBuilderChainNode<TContext, ReadOnlySequence<byte>> UseUInt32LengthFieldBasedFrameDecoder<TContext>(this InboundPipelineBuilderChainNode<TContext, PipeReader> builder)
        where TContext : IHoarwellContext
    {
        return builder.Use<UInt32LengthFieldBasedFrameDecoder<TContext>, ReadOnlySequence<byte>>(UInt32LengthFieldBasedFrameDecoder<TContext>.Shared);
    }

    /// <summary>
    /// 使用默认的基于 uin32 的帧编码器
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static OutboundPipelineBuilderChainNode<TContext, OutboundMetadata> UseUInt32LengthFieldBasedFrameEncoder<TContext>(this OutboundPipelineBuilderChainNode<TContext, OutboundMetadata> builder)
        where TContext : IHoarwellContext
    {
        return builder.Use<UInt32LengthFieldBasedFrameEncoder<TContext>, OutboundMetadata>(UInt32LengthFieldBasedFrameEncoder<TContext>.Shared);
    }

    #endregion UInt32

    #region CustomLength

    /// <summary>
    /// 使用<inheritdoc cref="LengthFieldBasedFrameDecoder{TContext}"/>
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="builder"></param>
    /// <param name="frameHeaderSize">帧头大小</param>
    /// <param name="shouldLengthIncludeFrameHeaderSize">长度是否包含帧头大小</param>
    /// <param name="useLittleEndian">是否使用小端序</param>
    /// <returns></returns>
    public static InboundPipelineBuilderChainNode<TContext, ReadOnlySequence<byte>> UseLengthFieldBasedFrameDecoder<TContext>(this InboundPipelineBuilderChainNode<TContext, PipeReader> builder,
                                                                                                                              int frameHeaderSize,
                                                                                                                              bool shouldLengthIncludeFrameHeaderSize = false,
                                                                                                                              bool useLittleEndian = true)
        where TContext : IHoarwellContext
    {
        var decoder = new LengthFieldBasedFrameDecoder<TContext>(frameHeaderSize, shouldLengthIncludeFrameHeaderSize, useLittleEndian);
        return builder.Use<LengthFieldBasedFrameDecoder<TContext>, ReadOnlySequence<byte>>(decoder);
    }

    /// <summary>
    /// 使用<inheritdoc cref="LengthFieldBasedFrameEncoder{TContext}"/>
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="builder"></param>
    /// <param name="frameHeaderSize">帧头大小</param>
    /// <param name="shouldLengthIncludeFrameHeaderSize">长度是否包含帧头大小</param>
    /// <param name="useLittleEndian">是否使用小端序</param>
    /// <returns></returns>
    public static OutboundPipelineBuilderChainNode<TContext, OutboundMetadata> UseLengthFieldBasedFrameEncoder<TContext>(this OutboundPipelineBuilderChainNode<TContext, OutboundMetadata> builder,
                                                                                                                         int frameHeaderSize,
                                                                                                                         bool shouldLengthIncludeFrameHeaderSize = false,
                                                                                                                         bool useLittleEndian = true)
        where TContext : IHoarwellContext
    {
        var encoder = new LengthFieldBasedFrameEncoder<TContext>(frameHeaderSize, shouldLengthIncludeFrameHeaderSize, useLittleEndian);
        return builder.Use<LengthFieldBasedFrameEncoder<TContext>, OutboundMetadata>(encoder);
    }

    #endregion CustomLength

    #endregion Public 方法
}
