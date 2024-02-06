using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using Hoarwell.Benchmark.DotNetty;

namespace Hoarwell.Benchmark;

internal class ServerBaseOnDotNetty
{
    #region Private 字段

    private IEventLoopGroup _boosGroup = null;

    private ServerBootstrap _bootstrap = null;

    private IChannel _serverChannel = null;

    private IEventLoopGroup _workerGroup = null;

    #endregion Private 字段

    #region Public 构造函数

    public ServerBaseOnDotNetty()
    {
    }

    #endregion Public 构造函数

    #region Public 方法

    public async ValueTask DisposeAsync()
    {
        await _serverChannel.CloseAsync();
        await _boosGroup.ShutdownGracefullyAsync();
        await _workerGroup.ShutdownGracefullyAsync();
    }

    public async Task StartAsync(int port)
    {
        _boosGroup = new DispatcherEventLoopGroup();
        _workerGroup = new WorkerEventLoopGroup(_boosGroup as DispatcherEventLoopGroup);
        _bootstrap = new ServerBootstrap();

        _bootstrap
            .Group(_boosGroup, _workerGroup)
            .Channel<TcpServerChannel>()
            .Option(ChannelOption.SoBacklog, 200)
            .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
            {
                var pipeLine = channel.Pipeline;
                pipeLine.AddLast("fm_encoder", new LengthFieldPrepender(2));
                pipeLine.AddLast("fm_decoder", new LengthFieldBasedFrameDecoder(100 * 1024, 0, 2, 0, 2));
                pipeLine.AddLast("buffer_encoder", new ByteBufferCodecEncoder());
                pipeLine.AddLast("decoder", new MessageDecoder());
                pipeLine.AddLast("echo", new DotNettyEchoDataServerMessageHandler());
            }));

        _serverChannel = await _bootstrap.BindAsync(port);
    }

    #endregion Public 方法
}
