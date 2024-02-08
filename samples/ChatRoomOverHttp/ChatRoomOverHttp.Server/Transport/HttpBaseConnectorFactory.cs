using System.Runtime.CompilerServices;
using Hoarwell;

namespace ChatRoomOverHttp.Server.Transport;

internal class HttpBaseConnectorFactory : IDuplexPipeConnectorFactory<Stream, Stream>
{
    #region Private 字段

    private readonly HttpBasePipeConnector _httpBaseConnector;

    #endregion Private 字段

    #region Public 构造函数

    public HttpBaseConnectorFactory(HttpBasePipeConnector httpBaseConnector)
    {
        _httpBaseConnector = httpBaseConnector ?? throw new ArgumentNullException(nameof(httpBaseConnector));
    }

    #endregion Public 构造函数

    #region Public 方法

    public async IAsyncEnumerable<IDuplexPipeConnector<Stream, Stream>> GetAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        yield return _httpBaseConnector;
    }

    #endregion Public 方法
}
