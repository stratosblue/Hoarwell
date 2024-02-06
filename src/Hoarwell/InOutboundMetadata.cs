using Hoarwell.Enhancement.Buffers;

namespace Hoarwell;

/// <summary>
/// 出站元数据
/// </summary>
/// <param name="BufferWriter">buffer writer</param>
/// <param name="Value">出站对象</param>
/// <param name="ValueType">出站对象类型</param>
public record struct OutboundMetadata(IRandomAccessibleBufferWriter<byte> BufferWriter, object? Value, Type ValueType);

/// <summary>
/// 入站元数据
/// </summary>
/// <param name="Value">入站对象</param>
/// <param name="ValueType">入站对象类型</param>
public record struct InboundMetadata(object? Value, Type ValueType);
