using System.Net;
using Hoarwell.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hoarwell.Client;

internal class DefaultSocketStreamClientConnectorFactory
    : SingleEndPointDuplexPipeConnectorFactory<Stream, Stream>
{
    #region Private 字段

    private readonly SocketCreateOptions _socketCreateOptions;

    #endregion Private 字段

    #region Public 构造函数

    public DefaultSocketStreamClientConnectorFactory([ServiceKey] string applicationName,
                                                     IOptionsMonitor<HoarwellEndPointOptions> endPointOptionsMonitor,
                                                     IOptionsMonitor<SocketCreateOptions> socketCreateOptionsMonitor)
        : base(applicationName, endPointOptionsMonitor)
    {
        _socketCreateOptions = socketCreateOptionsMonitor.GetRequiredApplicationOptions(applicationName, static options => options);
    }

    #endregion Public 构造函数

    #region Protected 方法

    protected override IDuplexPipeConnector<Stream, Stream> CreateConnector(EndPoint endPoint)
    {
        return new DefaultSocketStreamClientConnector(endPoint, _socketCreateOptions);
    }

    #endregion Protected 方法
}
