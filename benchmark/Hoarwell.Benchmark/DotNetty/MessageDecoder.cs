using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Hoarwell.Benchmark.DotNetty;

public class MessageDecoder : ByteToMessageDecoder
{
    #region 方法

    protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
    {
        if (input.ReadableBytes > 1)
        {
            var messageType = input.ReadShortLE();
            IByteBufferCodec obj = null;

            switch (messageType)
            {
                case EchoData.TypeId:
                    obj = new EchoData();
                    break;
                default:
                    break;
            }

            if (obj != null)
            {
                output.Add(obj.Decode(input));
            }
        }
    }

    #endregion 方法
}
