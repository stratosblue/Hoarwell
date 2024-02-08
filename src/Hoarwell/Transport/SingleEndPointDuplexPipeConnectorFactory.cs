using System.Net;
using Hoarwell.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hoarwell.Transport;

/// <summary>
/// 单远程终结点 <inheritdoc cref="IDuplexPipeConnectorFactory{TInputter, TOutputter}"/>, 仅连接一次远程终结点
/// </summary>
/// <typeparam name="TInputter"></typeparam>
/// <typeparam name="TOutputter"></typeparam>
public abstract class SingleEndPointDuplexPipeConnectorFactory<TInputter, TOutputter>
    : SingletonConnectorDuplexPipeConnectorFactory<TInputter, TOutputter>
{
    #region Protected 属性

    /// <summary>
    /// 远程 <inheritdoc cref="EndPoint"/>
    /// </summary>
    protected EndPoint RemoteEndPoint { get; }

    #endregion Protected 属性

    #region Public 构造函数

    /// <inheritdoc cref="SingleEndPointDuplexPipeConnectorFactory{TInputter, TOutputter}"/>
    public SingleEndPointDuplexPipeConnectorFactory([ServiceKey] string applicationName,
                                                    IOptionsMonitor<HoarwellEndPointOptions> optionsMonitor)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(applicationName);

        var endPoints = optionsMonitor.Get(applicationName)?.EndPoints;

        if (endPoints is null
            || endPoints.Count != 1)
        {
            throw new ArgumentException($"{GetType()} require only one endpoint in {nameof(HoarwellEndPointOptions)}.");
        }

        RemoteEndPoint = endPoints.First();
    }

    #endregion Public 构造函数

    #region Protected 方法

    /// <summary>
    /// 使用 <paramref name="endPoint"/> 创建连接器
    /// </summary>
    /// <param name="endPoint"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract ValueTask<IDuplexPipeConnector<TInputter, TOutputter>> CreateConnectorAsync(EndPoint endPoint, CancellationToken cancellationToken);

    /// <inheritdoc/>
    protected override ValueTask<IDuplexPipeConnector<TInputter, TOutputter>> CreateConnectorAsync(CancellationToken cancellationToken)
    {
        return CreateConnectorAsync(RemoteEndPoint, cancellationToken);
    }

    #endregion Protected 方法
}
