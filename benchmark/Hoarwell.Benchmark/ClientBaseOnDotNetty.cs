using System.Net;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Hoarwell.Benchmark.DotNetty;

namespace Hoarwell.Benchmark;

internal class ClientBaseOnDotNetty
{
    #region Private 字段

    private Bootstrap _bootstrap = null;

    private IChannel _channel = null;

    private DotNettyEchoDataClientMessageHandler _handler = null;

    private IEventLoopGroup _workerGroup = null;

    #endregion Private 字段

    #region Public 构造函数

    public ClientBaseOnDotNetty()
    {
    }

    #endregion Public 构造函数

    #region Public 方法

    public async ValueTask DisposeAsync()
    {
        await _channel.CloseAsync();
        await _workerGroup.ShutdownGracefullyAsync();
    }

    public async Task StartAsync(int port, int echoCount)
    {
        _workerGroup = new MultithreadEventLoopGroup();
        _bootstrap = new Bootstrap();

        _bootstrap
            .Group(_workerGroup)
            .Channel<TcpSocketChannel>()
            .Handler(new ActionChannelInitializer<IChannel>(channel =>
            {
                var pipeLine = channel.Pipeline;
                pipeLine.AddLast("fm_encoder", new LengthFieldPrepender(2));
                pipeLine.AddLast("fm_decoder", new LengthFieldBasedFrameDecoder(100 * 1024, 0, 2, 0, 2));
                pipeLine.AddLast("buffer_encoder", new ByteBufferCodecEncoder());
                pipeLine.AddLast("decoder", new MessageDecoder());
                _handler = new DotNettyEchoDataClientMessageHandler(echoCount);
                pipeLine.AddLast("echo", _handler);
            }));

        _channel = await _bootstrap.ConnectAsync(IPEndPoint.Parse($"127.0.0.1:{port}"));
    }

    public async Task WaitCompleteAndResetAsync()
    {
        await _handler.TaskCompletionSource.Task;
        _handler.Reset();
    }

    public Task WriteAndFlushAsync(object message)
    {
        return _channel.WriteAndFlushAsync(message);
    }

    #endregion Public 方法
}
