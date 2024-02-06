using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Hoarwell;

namespace Chat.Shared;

public class ChatPacket
    : IBinaryParseable<ChatPacket>, IBinarizable, ITypeIdentifierProvider
{
    #region Public 字段

    public const short TypeId = 1;

    #endregion Public 字段

    #region Public 属性

    public static ReadOnlySpan<byte> TypeIdentifier => BitConverter.GetBytes(TypeId);

    public string Message { get; set; }

    public string Name { get; set; }

    #endregion Public 属性

    #region Public 方法

    public static bool TryParse(in ReadOnlySequence<byte> input, [MaybeNullWhen(false)] out ChatPacket result)
    {
        var reader = new SequenceReader<byte>(input);

        reader.TryReadLittleEndian(out int nameLength);
        var name = Encoding.UTF8.GetString(reader.UnreadSequence.Slice(0, nameLength));
        reader.Advance(nameLength);

        reader.TryReadLittleEndian(out int messageLength);
        var message = Encoding.UTF8.GetString(reader.UnreadSequence.Slice(0, messageLength));

        result = new()
        {
            Name = name,
            Message = message,
        };

        return true;
    }

    public void Serialize(in IBufferWriter<byte> bufferWriter)
    {
        var nameData = Encoding.UTF8.GetBytes(Name);
        bufferWriter.Write(BitConverter.GetBytes(nameData.Length));
        bufferWriter.Write(nameData);

        var messageData = Encoding.UTF8.GetBytes(Message);
        bufferWriter.Write(BitConverter.GetBytes(messageData.Length));
        bufferWriter.Write(messageData);
    }

    #endregion Public 方法
}
