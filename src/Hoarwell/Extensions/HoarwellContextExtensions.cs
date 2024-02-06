using System.ComponentModel;
using Hoarwell;

namespace Microsoft.Extensions.DependencyInjection;

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
