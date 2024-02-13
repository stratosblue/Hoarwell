using DotNetty.Transport.Channels;

namespace Hoarwell.Benchmark.DotNetty;

internal class DotNettyEchoDataServerMessageHandler : SimpleChannelInboundHandler<EchoData>
{
    #region Protected 方法

    protected override void ChannelRead0(IChannelHandlerContext ctx, EchoData msg)
    {
        var echo = new EchoData()
        {
            Id = msg.Id * 2,
            Data = msg.Data,
        };

        ctx.WriteAndFlushAsync(echo);
    }

    #endregion Protected 方法
}
