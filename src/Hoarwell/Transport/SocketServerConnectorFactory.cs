using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Hoarwell.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hoarwell.Transport;

/// <summary>
/// 基于 <see cref="Socket"/> 的 <inheritdoc cref="IDuplexPipeConnectorFactory{TInputter, TOutputter}"/>
/// </summary>
public class SocketServerConnectorFactory : IDuplexPipeConnectorFactory<Stream, Stream>
{
    #region Private 字段

    private readonly IReadOnlyList<EndPoint> _endPoints;

    private readonly ILogger _logger;

    private readonly SocketCreateDelegate _socketCreateDelegate;

    #endregion Private 字段

    #region Public 构造函数

    /// <inheritdoc cref="SocketServerConnectorFactory"/>
    public SocketServerConnectorFactory([ServiceKey] string applicationName,
                                        IOptionsMonitor<HoarwellEndPointOptions> endPointOptionsMonitor,
                                        IOptionsMonitor<SocketCreateOptions> socketCreateOptionsMonitor,
                                        IServiceProvider serviceProvider,
                                        ILogger<SocketServerConnectorFactory> logger)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(applicationName, nameof(applicationName));
        ArgumentNullExceptionHelper.ThrowIfNull(serviceProvider, nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var endPoints = endPointOptionsMonitor.GetRequiredApplicationOptions(applicationName, m => m.EndPoints);

        if (endPoints.Count == 0)
        {
            throw new ArgumentException($"{nameof(SocketServerConnectorFactory)} requires at least one endpoint in {nameof(HoarwellEndPointOptions)}.");
        }

        _socketCreateDelegate = socketCreateOptionsMonitor.GetRequiredApplicationOptions(applicationName, m => m.SocketCreateFactory ?? SocketCreateOptions.DefaultSocketCreateFactory);

        _endPoints = endPoints.ToArray();
    }

    #endregion Public 构造函数

    #region Public 方法

#pragma warning disable CS1998

    /// <inheritdoc/>
    public async IAsyncEnumerable<IDuplexPipeConnector<Stream, Stream>> GetAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var endPoint in _endPoints)
        {
            var resolvedEndPoint = await ResolveEndPointAsync(endPoint, cancellationToken).ConfigureAwait(false);

            var socket = _socketCreateDelegate(resolvedEndPoint);

            try
            {
                socket.Bind(resolvedEndPoint);

                //TODO backlog optionable
                socket.Listen(int.MaxValue);
            }
            catch
            {
                socket.Dispose();
                throw;
            }

            yield return new SocketListenerConnector(socket);
        }
    }

    #endregion Public 方法

    #region Protected 方法

    /// <summary>
    /// 处理端点
    /// </summary>
    /// <param name="endPoint"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task<EndPoint> ResolveEndPointAsync(EndPoint endPoint, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting resolve endpoint {EndPoint}.", endPoint);

        var result = await EndPointResolveHelper.ResolveForSocketAsync(endPoint, cancellationToken).ConfigureAwait(false);

        if (result != endPoint)
        {
            _logger.LogInformation("Endpoint {EndPoint} resolved as {NewValue}.", endPoint, result);
        }
        else
        {
            _logger.LogDebug("Endpoint {EndPoint} does not need to be resolved.", endPoint);
        }

        return result;
    }

    #endregion Protected 方法
}
