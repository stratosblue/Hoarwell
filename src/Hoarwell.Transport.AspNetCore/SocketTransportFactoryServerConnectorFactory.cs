using System.IO.Pipelines;
using System.Net;
using System.Runtime.CompilerServices;
using Hoarwell.Options;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hoarwell.Transport.AspNetCore;

/// <summary>
/// 基于 <see cref="IConnectionListenerFactory"/> 的 <inheritdoc cref="IDuplexPipeConnectorFactory{TInputter, TOutputter}"/>
/// </summary>
public class SocketTransportFactoryServerConnectorFactory : IDuplexPipeConnectorFactory<PipeReader, PipeWriter>
{
    #region Private 字段

    private readonly IConnectionListenerFactory _connectionListenerFactory;

    private readonly IReadOnlyList<EndPoint> _endPoints;

    #endregion Private 字段

    #region Public 构造函数

    /// <inheritdoc cref="SocketTransportFactoryServerConnectorFactory"/>
    public SocketTransportFactoryServerConnectorFactory([ServiceKey] string applicationName,
                                                        IOptionsMonitor<HoarwellEndPointOptions> optionsMonitor,
                                                        IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(applicationName);

        _connectionListenerFactory = serviceProvider.GetRequiredKeyedService<IConnectionListenerFactory>(applicationName);

        var endPoints = optionsMonitor.Get(applicationName)?.EndPoints;

        if (endPoints is null
            || endPoints.Count == 0)
        {
            throw new ArgumentException($"{nameof(IDuplexPipeConnectorFactory<PipeReader, PipeWriter>)} requires at least one endpoint in {nameof(HoarwellEndPointOptions)}.");
        }

        _endPoints = endPoints.ToArray();
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public async IAsyncEnumerable<IDuplexPipeConnector<PipeReader, PipeWriter>> GetAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var endPoint in _endPoints)
        {
            var connectionListener = await _connectionListenerFactory.BindAsync(endPoint, cancellationToken).ConfigureAwait(false);

            yield return new DuplexPipeConnectionListenerConnector(connectionListener);
        }
    }

    #endregion Public 方法
}
