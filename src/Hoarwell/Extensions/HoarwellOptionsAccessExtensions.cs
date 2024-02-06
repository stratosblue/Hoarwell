using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;

namespace Hoarwell.Extensions;

/// <summary>
/// Hoarwell Options 访问拓展方法集合
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class HoarwellOptionsAccessExtensions
{
    #region Public 方法

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="optionsMonitor"></param>
    /// <param name="applicationName"></param>
    /// <param name="resultAccessFunc"></param>
    /// <param name="exceptionMessage"></param>
    /// <param name="resultAccessExpression"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    [return: NotNull]
    public static TResult GetRequiredApplicationOptions<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TOptions, TResult>(this IOptionsMonitor<TOptions>? optionsMonitor,
                                                                                                                                                                       string applicationName,
                                                                                                                                                                       Func<TOptions, TResult> resultAccessFunc,
                                                                                                                                                                       string? exceptionMessage = null,
                                                                                                                                                                       [CallerArgumentExpression(nameof(resultAccessFunc))] string? resultAccessExpression = null)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(optionsMonitor);
        ArgumentNullExceptionHelper.ThrowIfNull(applicationName);

        if (optionsMonitor.Get(applicationName) is not { } options
            || resultAccessFunc(options) is not { } result)
        {
            var message = exceptionMessage ?? $"Can not get options - {typeof(TOptions)} by \"{resultAccessExpression}\" for application \"{applicationName}\"";

            throw new ArgumentException(message);
        }
        return result;
    }

    #endregion Public 方法
}
