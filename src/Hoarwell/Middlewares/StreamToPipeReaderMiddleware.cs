using System.IO.Pipelines;

namespace Hoarwell.Middlewares;

/// <summary>
/// </summary>
/// <typeparam name="TContext"></typeparam>
internal sealed class StreamToPipeReaderMiddleware<TContext> : TransformMiddleware<TContext, Stream, PipeReader>
    where TContext : IHoarwellContext
{
    #region Public 属性

    /// <summary>
    /// 共享实例
    /// </summary>
    public static StreamToPipeReaderMiddleware<TContext> Shared { get; } = new();

    #endregion Public 属性

    #region Public 方法

    public override Task<PipeReader> InvokeAsync(TContext context, Stream input)
    {
        var reader = PipeReader.Create(input);
        return Task.FromResult(reader);
    }

    #endregion Public 方法
}
