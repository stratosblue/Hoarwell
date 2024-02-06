using System.Net;
using Hoarwell.Enhancement.Collections;
using Hoarwell.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hoarwell.Client;

/// <summary>
/// 单远程终结点 <inheritdoc cref="IDuplexPipeConnectorFactory{TInputter, TOutputter}"/>, 仅连接一次远程终结点
/// </summary>
/// <typeparam name="TInputter"></typeparam>
/// <typeparam name="TOutputter"></typeparam>
public abstract class SingleEndPointDuplexPipeConnectorFactory<TInputter, TOutputter>
    : IDuplexPipeConnectorFactory<TInputter, TOutputter>
{
    #region Private 字段

    private readonly object _syncRoot = new();

    #endregion Private 字段

    #region Protected 字段

    /// <summary>
    /// 远程 <inheritdoc cref="EndPoint"/>
    /// </summary>
    protected EndPoint RemoteEndPoint { get; }

    #endregion Protected 字段

    #region Protected 属性

    /// <summary>
    /// 连接器
    /// </summary>
    protected IDuplexPipeConnector<TInputter, TOutputter>? Connector { get; private set; }

    /// <summary>
    /// 连接器枚举器
    /// </summary>
    protected IAsyncEnumerable<IDuplexPipeConnector<TInputter, TOutputter>>? ConnectorAsyncEnumerable { get; private set; }

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

    #region Public 方法

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<IDuplexPipeConnector<TInputter, TOutputter>> GetAsync(CancellationToken cancellationToken = default)
    {
        if (ConnectorAsyncEnumerable is not null)
        {
            return ConnectorAsyncEnumerable;
        }

        lock (_syncRoot)
        {
            if (ConnectorAsyncEnumerable is not null)
            {
                return ConnectorAsyncEnumerable;
            }

            Connector = CreateConnector(RemoteEndPoint);
            ConnectorAsyncEnumerable = new SingleItemAsyncEnumerable<IDuplexPipeConnector<TInputter, TOutputter>>(Connector);

            return ConnectorAsyncEnumerable;
        }
    }

    #endregion Public 方法

    #region Protected 方法

    /// <summary>
    /// 使用 <paramref name="endPoint"/> 创建连接器
    /// </summary>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    protected abstract IDuplexPipeConnector<TInputter, TOutputter> CreateConnector(EndPoint endPoint);

    #endregion Protected 方法
}
