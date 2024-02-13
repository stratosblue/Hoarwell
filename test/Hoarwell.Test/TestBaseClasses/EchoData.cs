using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Hoarwell.Enhancement.Buffers;

namespace Hoarwell;

public class EchoData
    :
#if NET7_0_OR_GREATER
     IBinaryParseable<EchoData>, ITypeIdentifierProvider,
#endif
     IBinarizable
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
        bufferWriter.WriteLittleEndian(Id);
        var messageData = Encoding.UTF8.GetBytes(Message);
        bufferWriter.WriteLittleEndian(messageData.Length);
        bufferWriter.Write(messageData);
    }

    #endregion Public 方法
}
