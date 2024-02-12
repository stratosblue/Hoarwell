using System.Diagnostics;
using Hoarwell.Build;

namespace Hoarwell.Extensions;

/// <summary>
/// <see cref="HoarwellEndpointBuilder"/> 拓展方法集合
/// </summary>
public static class HoarwellEndpointBuilderExtensions
{
    #region Public 方法

    /// <summary>
    /// 使用指定委托 <paramref name="handleDelegate"/> 处理类型为 <typeparamref name="TMessage"/> 的消息
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="builder"></param>
    /// <param name="handleDelegate"></param>
    /// <returns></returns>
    public static HoarwellEndpointBuilder Handle<TMessage>(this HoarwellEndpointBuilder builder,
                                                           MessageHandleDelegate<TMessage> handleDelegate)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(builder);
        ArgumentNullExceptionHelper.ThrowIfNull(handleDelegate);

        builder.HandleInboundMessageDelegateMap.Add(typeof(TMessage), [DebuggerStepThrough][StackTraceHidden] (IHoarwellContext context, InboundMetadata input) => handleDelegate(context, (TMessage?)input.Value));

        return builder;
    }

    #endregion Public 方法
}
