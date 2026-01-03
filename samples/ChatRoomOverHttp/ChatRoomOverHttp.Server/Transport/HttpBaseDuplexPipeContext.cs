using Hoarwell;
using Hoarwell.Features;
using Microsoft.AspNetCore.Http;

namespace ChatRoomOverHttp.Server.Transport;

internal class HttpBaseDuplexPipeContext : IDuplexPipeContext<Stream, Stream>
{
    #region Private 字段

    private readonly HttpContext _context;

    #endregion Private 字段

    #region Public 属性

    public IFeatureCollection Features { get; }

    public Stream Inputter { get; }

    public Stream Outputter { get; }

    public CancellationToken PipeClosed { get; }

    #endregion Public 属性

    #region Public 构造函数

    public HttpBaseDuplexPipeContext(HttpContext context)
    {
        _context = context;
        PipeClosed = context.RequestAborted;
        Features = new ConcurrentFeatureCollection(context.Features!);
        Inputter = context.Request.Body;
        Outputter = context.Response.Body;

        Features.Set<IDuplexPipeFeature<Stream, Stream>>(this);
        Features.Set<IPipeLifetimeFeature>(this);
    }

    #endregion Public 构造函数

    #region Public 方法

    public void Abort()
    {
        _context.Abort();
    }

    public ValueTask DisposeAsync()
    {
        _context.Abort();
        return default;
    }

    #endregion Public 方法
}
