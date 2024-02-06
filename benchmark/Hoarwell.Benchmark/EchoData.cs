using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using DotNetty.Buffers;
using Hoarwell.Benchmark.DotNetty;

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

    public int Id { get; set; }

    public string Message { get; set; }

    #endregion Public 属性

    #region Public 方法

    public static bool TryParse(in ReadOnlySequence<byte> input, [MaybeNullWhen(false)] out EchoData result)
    {
        var reader = new SequenceReader<byte>(input);

        reader.TryReadLittleEndian(out int id);
        reader.TryReadLittleEndian(out int nameLength);
        var name = Encoding.UTF8.GetString(reader.UnreadSequence.Slice(0, nameLength));

        result = new()
        {
            Id = id,
            Message = name,
        };

        return true;
    }

    public void Serialize(in IBufferWriter<byte> bufferWriter)
    {
        bufferWriter.Write(BitConverter.GetBytes(Id));
        var nameData = Encoding.UTF8.GetBytes(Message);
        bufferWriter.Write(BitConverter.GetBytes(nameData.Length));
        bufferWriter.Write(nameData);
    }

    #region IByteBufferCodec

    public object Decode(IByteBuffer buffer)
    {
        Id = buffer.ReadIntLE();
        var nameLength = buffer.ReadIntLE();
        Message = buffer.ReadString(nameLength, Encoding.UTF8);

        return this;
    }

    public IByteBuffer Encode(IByteBuffer buffer)
    {
        buffer.WriteIntLE(Id);
        var nameData = Encoding.UTF8.GetBytes(Message);
        buffer.WriteIntLE(nameData.Length);
        buffer.WriteBytes(nameData);
        return buffer;
    }

    #endregion IByteBufferCodec

    #endregion Public 方法
}
