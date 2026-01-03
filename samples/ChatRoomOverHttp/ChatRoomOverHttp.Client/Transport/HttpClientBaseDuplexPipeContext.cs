using ChatRoomOverHttp.Client.Transport;
using Hoarwell;
using Hoarwell.Features;

namespace ChatRoomOverHttp.Server.Transport;

internal class HttpClientBaseDuplexPipeContext : IDuplexPipeContext<Stream, Stream>
{
    #region Private 字段

    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly HttpClient _httpClient;

    private readonly Task<Stream> _responseStreamGetTask;

    #endregion Private 字段

    #region Public 属性

    public IFeatureCollection Features { get; }

    public Stream Inputter { get; }

    public Stream Outputter { get; }

    public CancellationToken PipeClosed { get; }

    #endregion Public 属性

    #region Public 构造函数

    public HttpClientBaseDuplexPipeContext(HttpClient httpClient, Task<Stream> responseStreamGetTask, Stream writeStream, CancellationTokenSource cancellationTokenSource)
    {
        _httpClient = httpClient;
        _responseStreamGetTask = responseStreamGetTask;
        PipeClosed = cancellationTokenSource.Token;
        Features = new ConcurrentFeatureCollection();
        Inputter = new DelayInitStream(responseStreamGetTask);
        Outputter = writeStream;

        _cancellationTokenSource = cancellationTokenSource;

        Features.Set<IDuplexPipeFeature<Stream, Stream>>(this);
        Features.Set<IPipeLifetimeFeature>(this);
    }

    #endregion Public 构造函数

    #region Public 方法

    public void Abort()
    {
        _cancellationTokenSource.Cancel();
    }

    public ValueTask DisposeAsync()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        return default;
    }

    #endregion Public 方法
}
