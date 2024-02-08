using System.Threading.Channels;
using Hoarwell;
using Microsoft.AspNetCore.Http.Features;

namespace ChatRoomOverHttp.Server.Transport;

internal class HttpBasePipeConnector : IDuplexPipeConnector<Stream, Stream>
{
    #region Private 字段

    private readonly Channel<IDuplexPipeContext<Stream, Stream>> _contextChannel;

    #endregion Private 字段

    #region Public 属性

    public IFeatureCollection Features { get; } = new FeatureCollection();

    #endregion Public 属性

    #region Public 构造函数

    public HttpBasePipeConnector()
    {
        _contextChannel = Channel.CreateUnbounded<IDuplexPipeContext<Stream, Stream>>();
    }

    #endregion Public 构造函数

    #region Public 方法

    public ValueTask<IDuplexPipeContext<Stream, Stream>> ConnectAsync(CancellationToken cancellationToken = default)
    {
        return _contextChannel.Reader.ReadAsync(cancellationToken);
    }

    public async ValueTask<CancellationToken> ConnectAsync(Microsoft.AspNetCore.Http.HttpContext context)
    {
        var duplexPipeContext = new HttpBaseDuplexPipeContext(context);
        await _contextChannel.Writer.WriteAsync(duplexPipeContext, default);
        return duplexPipeContext.PipeClosed;
    }

    public ValueTask DisposeAsync()
    {
        //需要正确处置
        return default;
    }

    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        //需要正确停止
        return default;
    }

    #endregion Public 方法
}
