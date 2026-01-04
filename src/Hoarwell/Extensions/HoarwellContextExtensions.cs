using System.ComponentModel;

namespace Hoarwell;

/// <summary>
/// HoarwellContext 拓展方法集合
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class HoarwellContextExtensions
{
    #region Public 方法

    /// <inheritdoc cref="IOutputter.FlushAsync(CancellationToken)"/>
    public static Task FlushAsync<T>(this IHoarwellContext context, CancellationToken cancellationToken = default)
    {
        return context.Outputter.FlushAsync(cancellationToken);
    }

    /// <summary>
    /// 从<paramref name="context"/>中获取指定的<typeparamref name="TFeature"/>, 获取失败则抛出异常
    /// </summary>
    /// <typeparam name="TFeature"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static TFeature RequiredFeature<TFeature>(this IHoarwellContext context) where TFeature : class
    {
        return context.Features.Get<TFeature>() ?? throw new KeyNotFoundException($"Not found {typeof(TFeature)} in feature collection.");
    }

    /// <inheritdoc cref="IOutputter.WriteAndFlushAsync{T}(IHoarwellContext, T, CancellationToken)"/>
    public static Task WriteAndFlushAsync<T>(this IHoarwellContext context, T message, CancellationToken cancellationToken = default)
    {
        return context.Outputter.WriteAndFlushAsync(context, message, cancellationToken);
    }

    /// <inheritdoc cref="IOutputter.WriteAndFlushAsync(ReadOnlyMemory{byte}, CancellationToken)"/>
    public static Task WriteAndFlushAsync(this IHoarwellContext context, ReadOnlyMemory<byte> rawMessage, CancellationToken cancellationToken = default)
    {
        return context.Outputter.WriteAndFlushAsync(context, rawMessage, cancellationToken);
    }

    /// <inheritdoc cref="IOutputter.WriteAsync{T}(IHoarwellContext, T, CancellationToken)"/>
    public static Task WriteAsync<T>(this IHoarwellContext context, T message, CancellationToken cancellationToken = default)
    {
        return context.Outputter.WriteAsync(context, message, cancellationToken);
    }

    /// <inheritdoc cref="IOutputter.WriteAsync(ReadOnlyMemory{byte}, CancellationToken)"/>
    public static Task WriteAsync(this IHoarwellContext context, ReadOnlyMemory<byte> rawMessage, CancellationToken cancellationToken = default)
    {
        return context.Outputter.WriteAsync(context, rawMessage, cancellationToken);
    }

    #endregion Public 方法
}
