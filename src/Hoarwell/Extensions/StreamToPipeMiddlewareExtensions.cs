using System.ComponentModel;
using System.IO.Pipelines;
using Hoarwell;
using Hoarwell.Build;
using Hoarwell.Middlewares;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 管道适配 <see cref="Stream"/> 到 <see cref="PipeReader"/> 拓展方法集合
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class StreamToPipeMiddlewareExtensions
{
    #region Public 方法

    /// <summary>
    /// 使用 Pipe 适配中间件，将管道从 <see cref="Stream"/> 适配到 <see cref="PipeReader"/>
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static InboundPipelineBuilderChainNode<TContext, PipeReader> UsePipeReaderAdaptMiddleware<TContext>(this InboundPipelineBuilderChainNode<TContext, Stream> builder)
        where TContext : IHoarwellContext
    {
        return builder.Use<StreamToPipeReaderMiddleware<TContext>, PipeReader>(StreamToPipeReaderMiddleware<TContext>.Shared);
    }

    #endregion Public 方法
}
