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
    /// 使用默认的基于 uint32 的帧解码器
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
    /// 使用默认的基于 uint32 的帧编码器
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
    /// <param name="options">选项</param>
    /// <returns></returns>
    public static InboundPipelineBuilderChainNode<TContext, ReadOnlySequence<byte>> UseLengthFieldBasedFrameDecoder<TContext>(this InboundPipelineBuilderChainNode<TContext, PipeReader> builder,
                                                                                                                              LengthFieldBasedFrameCodecOptions options)
        where TContext : IHoarwellContext
    {
        var decoder = new LengthFieldBasedFrameDecoder<TContext>(options);
        return builder.Use<LengthFieldBasedFrameDecoder<TContext>, ReadOnlySequence<byte>>(decoder);
    }

    /// <summary>
    /// 使用<inheritdoc cref="LengthFieldBasedFrameEncoder{TContext}"/>
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="builder"></param>
    /// <param name="options">选项</param>
    /// <returns></returns>
    public static OutboundPipelineBuilderChainNode<TContext, OutboundMetadata> UseLengthFieldBasedFrameEncoder<TContext>(this OutboundPipelineBuilderChainNode<TContext, OutboundMetadata> builder,
                                                                                                                         LengthFieldBasedFrameCodecOptions options)
        where TContext : IHoarwellContext
    {
        var encoder = new LengthFieldBasedFrameEncoder<TContext>(options);
        return builder.Use<LengthFieldBasedFrameEncoder<TContext>, OutboundMetadata>(encoder);
    }

    #endregion CustomLength

    #endregion Public 方法
}
