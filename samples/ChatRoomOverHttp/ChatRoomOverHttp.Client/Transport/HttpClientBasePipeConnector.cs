using System.Net;
using ChatRoomOverHttp.Client.Transport;
using Hoarwell;
using Hoarwell.Transport;

namespace ChatRoomOverHttp.Server.Transport;

internal class HttpClientBasePipeConnector : SingleConnectionDuplexPipeConnector<Stream, Stream>
{
    #region Private 字段

    private readonly string _url;

    #endregion Private 字段

    #region Public 构造函数

    public HttpClientBasePipeConnector(string url)
    {
        _url = url;
    }

    #endregion Public 构造函数

    #region Protected 方法

    protected override async Task<IDuplexPipeContext<Stream, Stream>> InternalConnectAsync(CancellationToken cancellationToken)
    {
        var content = new LongAliveHttpContent();

        var httpRequestMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_url),
            Content = content,
            Version = HttpVersion.Version20,
            VersionPolicy = HttpVersionPolicy.RequestVersionExact,
        };

        var httpClient = new HttpClient()
        {
        };

        using var cts = new CancellationTokenSource();

        var responseStreamGetTask = httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead)
                                              .ContinueWith(m => m.Result.Content.ReadAsStreamAsync())
                                              .Unwrap();

        var writeStream = await content.GetStreamAsync();

        return new HttpClientBaseDuplexPipeContext(httpClient, responseStreamGetTask, writeStream, cts);
    }

    #endregion Protected 方法
}
