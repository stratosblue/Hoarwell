using System.Runtime.CompilerServices;
using Hoarwell.Enhancement.Collections;

namespace Hoarwell.Transport;

/// <summary>
/// 单例连接器的<inheritdoc cref="IDuplexPipeConnectorFactory{TInputter, TOutputter}"/>
/// </summary>
/// <typeparam name="TInputter"></typeparam>
/// <typeparam name="TOutputter"></typeparam>
public abstract class SingletonConnectorDuplexPipeConnectorFactory<TInputter, TOutputter>
    : IDuplexPipeConnectorFactory<TInputter, TOutputter>, IDisposable
{
    #region Private 字段

    private readonly SemaphoreSlim _createConnectorSemaphore = new(1, 1);

    private int _isDisposed = 0;

    #endregion Private 字段

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

    #region Public 方法

    /// <inheritdoc/>
    public virtual IAsyncEnumerable<IDuplexPipeConnector<TInputter, TOutputter>> GetAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (ConnectorAsyncEnumerable is not null)
        {
            return ConnectorAsyncEnumerable;
        }

        return GetOrCreateConnectorEnumerator(cancellationToken);
    }

    #endregion Public 方法

    #region Protected 方法

    /// <summary>
    /// 获取或创建连接枚举器
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async IAsyncEnumerable<IDuplexPipeConnector<TInputter, TOutputter>> GetOrCreateConnectorEnumerator([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        await _createConnectorSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (Connector is null)
            {
                ThrowIfDisposed();

                Connector = await CreateConnectorAsync(cancellationToken).ConfigureAwait(false);
                ConnectorAsyncEnumerable = new SingleItemAsyncEnumerable<IDuplexPipeConnector<TInputter, TOutputter>>(Connector);
            }
        }
        finally
        {
            _createConnectorSemaphore.Release();
        }
        yield return Connector;
    }

    #endregion Protected 方法

    #region Protected 方法

    /// <summary>
    /// 创建连接器
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract ValueTask<IDuplexPipeConnector<TInputter, TOutputter>> CreateConnectorAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 对象已处置时抛出异常
    /// </summary>
    protected void ThrowIfDisposed()
    {
        ObjectDisposedExceptionHelper.ThrowIf(_isDisposed == 1, this);
    }

    #region Dispose

    /// <summary>
    ///
    /// </summary>
    ~SingletonConnectorDuplexPipeConnectorFactory()
    {
        Dispose(disposing: false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
        {
            _createConnectorSemaphore.Dispose();
            Connector = null;
            ConnectorAsyncEnumerable = null;
        }
    }

    #endregion Dispose

    #endregion Protected 方法
}
