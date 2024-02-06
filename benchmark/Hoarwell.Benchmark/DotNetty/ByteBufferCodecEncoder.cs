using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Hoarwell.Benchmark.DotNetty;

public interface IByteBufferEncode
{
    #region 方法

    IByteBuffer Encode(IByteBuffer buffer);

    #endregion 方法
}

public class ByteBufferCodecEncoder : MessageToByteEncoder<IByteBufferEncode>
{
    #region 方法

    protected override void Encode(IChannelHandlerContext context, IByteBufferEncode message, IByteBuffer output)
    {
        switch (message)
        {
            case EchoData:
                output.WriteShortLE(EchoData.TypeId);
                break;

            default:
                break;
        }
        message.Encode(output);
    }

    #endregion 方法
}

public interface IByteBufferCodec : IByteBufferEncode
{
    #region 方法

    object Decode(IByteBuffer buffer);

    #endregion 方法
}
