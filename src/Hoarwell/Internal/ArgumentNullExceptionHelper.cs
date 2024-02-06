using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Hoarwell.Internal;

// .net低版本兼容代码

/// <summary>
/// ArgumentNullException.ThrowIfNull 兼容代码
/// </summary>
internal static class ArgumentNullExceptionHelper
{
    #region Public 方法

#if NET7_0_OR_GREATER

    /// <summary>
    /// <inheritdoc cref="ArgumentNullException.ThrowIfNull(object?, string?)"/>
    /// </summary>
    /// <param name="argument"></param>
    /// <param name="paramName"></param>
    public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
    }

#else

    /// <summary>
    /// ArgumentNullException.ThrowIfNull 兼容代码，当 <paramref name="argument"/> 为空时抛出异常，<paramref name="paramName"/> 需要手动传递
    /// </summary>
    /// <param name="argument"></param>
    /// <param name="paramName"></param>
    public static void ThrowIfNull<T>([NotNull] T? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName, $"Param type: {typeof(T)}");
        }
    }

#endif

    #endregion Public 方法
}
