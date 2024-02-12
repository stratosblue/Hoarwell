using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace Hoarwell;

/// <summary>
/// 处理入站消息委托
/// </summary>
/// <param name="context"></param>
/// <param name="input"></param>
/// <returns></returns>
public delegate Task HandleInboundMessageDelegate(IHoarwellContext context, InboundMetadata input);

/// <summary>
/// 消息处理委托
/// </summary>
/// <typeparam name="TMessage"></typeparam>
/// <param name="context"></param>
/// <param name="message"></param>
/// <returns></returns>
public delegate Task MessageHandleDelegate<TMessage>(IHoarwellContext context, TMessage? message);

/// <summary>
/// 消息输出序列化委托
/// </summary>
/// <param name="context"></param>
/// <param name="output"></param>
/// <returns></returns>
public delegate Task SerializeOutboundMessageDelegate(IHoarwellContext context, OutboundMetadata output);

/// <summary>
/// <see cref="Socket"/> 创建委托
/// </summary>
/// <param name="endPoint"></param>
/// <returns></returns>
public delegate Socket SocketCreateDelegate(EndPoint endPoint);

/// <summary>
/// 尝试从 <paramref name="input"/> 解析类型 <typeparamref name="T"/> 委托
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="input"></param>
/// <param name="result"></param>
/// <returns></returns>
public delegate bool TryBinaryParseDelegate<T>(in ReadOnlySequence<byte> input, [MaybeNullWhen(false)] out T? result);

#region ContextActive

/// <summary>
/// Hoarwell上下文激活回调
/// </summary>
/// <param name="context"></param>
public delegate void HoarwellContextActiveCallback(IHoarwellContext context);

/// <summary>
/// Hoarwell上下文失活回调
/// </summary>
/// <param name="context"></param>
public delegate void HoarwellContextInactiveCallback(IHoarwellContext context);

#endregion ContextActive
