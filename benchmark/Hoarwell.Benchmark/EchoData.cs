using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using DotNetty.Buffers;
using Hoarwell.Benchmark.DotNetty;
using Hoarwell.Enhancement.Buffers;

namespace Hoarwell.Benchmark;

public class EchoData
    : IBinaryParseable<EchoData>, IBinarizable, ITypeIdentifierProvider
    , IByteBufferCodec
{
    #region Public 字段

    public const short TypeId = 1;

    #endregion Public 字段

    #region Public 属性

    public static ReadOnlySpan<byte> TypeIdentifier => BitConverter.GetBytes(TypeId);

    public byte[] Data { get; set; }

    public int Id { get; set; }

    #endregion Public 属性

    #region Public 方法

    public static bool TryParse(in ReadOnlySequence<byte> input, [MaybeNullWhen(false)] out EchoData result)
    {
        var reader = new SequenceReader<byte>(input);

        reader.TryReadLittleEndian(out int id);
        reader.TryReadLittleEndian(out int dataLength);

        result = new()
        {
            Id = id,
            Data = reader.UnreadSequence.Slice(0, dataLength).ToArray(),
        };

        return true;
    }

    public void Serialize(in IBufferWriter<byte> bufferWriter)
    {
        bufferWriter.WriteLittleEndian(Id);
        bufferWriter.WriteLittleEndian(Data.Length);
        bufferWriter.Write(Data);
    }

    #region IByteBufferCodec

    public object Decode(IByteBuffer buffer)
    {
        Id = buffer.ReadIntLE();
        var dataLength = buffer.ReadIntLE();
        Data = new byte[dataLength];
        buffer.ReadBytes(Data);

        return this;
    }

    public IByteBuffer Encode(IByteBuffer buffer)
    {
        buffer.WriteIntLE(Id);
        buffer.WriteIntLE(Data.Length);
        buffer.WriteBytes(Data);
        return buffer;
    }

    #endregion IByteBufferCodec

    #endregion Public 方法
}
