using System.IO.Pipelines;
using System.Net;
using Hoarwell.Options;
using Hoarwell.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hoarwell.Client;

internal class DefaultSocketPipeClientConnectorFactory
    : SingleEndPointDuplexPipeConnectorFactory<PipeReader, PipeWriter>
{
    #region Private 字段

    private readonly SocketCreateOptions _socketCreateOptions;

    #endregion Private 字段

    #region Public 构造函数

    public DefaultSocketPipeClientConnectorFactory([ServiceKey] string applicationName,
                                                   IOptionsMonitor<HoarwellEndPointOptions> endPointOptionsMonitor,
                                                   IOptionsMonitor<SocketCreateOptions> socketCreateOptionsMonitor)
        : base(applicationName, endPointOptionsMonitor)
    {
        _socketCreateOptions = socketCreateOptionsMonitor.GetRequiredApplicationOptions(applicationName, static options => options);
    }

    #endregion Public 构造函数

    #region Protected 方法

    protected override ValueTask<IDuplexPipeConnector<PipeReader, PipeWriter>> CreateConnectorAsync(EndPoint endPoint, CancellationToken cancellationToken)
    {
        var connector = new DefaultSocketPipeClientConnector(endPoint, _socketCreateOptions);
        return new ValueTask<IDuplexPipeConnector<PipeReader, PipeWriter>>(connector);
    }

    #endregion Protected 方法
}
