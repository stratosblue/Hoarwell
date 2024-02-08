using System.Net;
using Hoarwell;
using Hoarwell.Options;
using Hoarwell.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ChatRoomOverHttp.Server.Transport;

internal class HttpClientBaseConnectorFactory : SingleEndPointDuplexPipeConnectorFactory<Stream, Stream>
{
    #region Public 构造函数

    public HttpClientBaseConnectorFactory([ServiceKey] string applicationName, IOptionsMonitor<HoarwellEndPointOptions> optionsMonitor) : base(applicationName, optionsMonitor)
    {
    }

    #endregion Public 构造函数

    #region Protected 方法

    protected override ValueTask<IDuplexPipeConnector<Stream, Stream>> CreateConnectorAsync(EndPoint endPoint, CancellationToken cancellationToken)
    {
        var connector = new HttpClientBasePipeConnector(endPoint.ToString()!);
        return ValueTask.FromResult<IDuplexPipeConnector<Stream, Stream>>(connector);
    }

    #endregion Protected 方法
}
