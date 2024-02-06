using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Hoarwell.Internal;

// .net低版本兼容代码

/// <summary>
/// ObjectDisposedException.ThrowIf 兼容代码
/// </summary>
internal class ObjectDisposedExceptionHelper
{
#if NET7_0_OR_GREATER

    /// <summary>
    /// <inheritdoc cref="ObjectDisposedException.ThrowIf(bool, object)"/>
    /// </summary>
    [StackTraceHidden]
    public static void ThrowIf([DoesNotReturnIf(true)] bool condition, object instance)
    {
        ObjectDisposedException.ThrowIf(condition, instance);
    }

#else

    #region Public 方法

    /// <summary>
    /// ObjectDisposedException.ThrowIf 兼容代码，当 <paramref name="condition"/> 为 true 时抛出异常
    /// </summary>
    public static void ThrowIf<T>([DoesNotReturnIf(true)] bool condition, T instance)
    {
        if (condition)
        {
            throw new ObjectDisposedException(typeof(T).Name);
        }
    }

    #endregion Public 方法

#endif
}
