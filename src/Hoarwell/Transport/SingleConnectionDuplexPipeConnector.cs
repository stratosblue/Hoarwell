using Hoarwell.Features;

namespace Hoarwell.Transport;

/// <summary>
/// 单次连接的 <inheritdoc cref="IDuplexPipeConnector{TInputter, TOutputter}"/><br/>
/// 仅进行一次连接，并阻塞 <see cref="ConnectAsync(CancellationToken)"/> 方法
/// </summary>
/// <typeparam name="TInputter"></typeparam>
/// <typeparam name="TOutputter"></typeparam>
public abstract class SingleConnectionDuplexPipeConnector<TInputter, TOutputter>
    : IDuplexPipeConnector<TInputter, TOutputter>, IDisposable
{
    #region Private 字段

    private readonly CancellationTokenSource _blockTokenSource = new();

    private TaskCompletionSource<IDuplexPipeContext<TInputter, TOutputter>>? _completionSource;

    private int _isDisposed = 0;

    #endregion Private 字段

    #region Public 属性

    /// <inheritdoc/>
    public virtual IFeatureCollection Features { get; } = new ConcurrentFeatureCollection();

    #endregion Public 属性

    #region Public 方法

    /// <inheritdoc/>
    public virtual async ValueTask<IDuplexPipeContext<TInputter, TOutputter>> ConnectAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        var newCompletionSource = new TaskCompletionSource<IDuplexPipeContext<TInputter, TOutputter>>();
        if (Interlocked.CompareExchange(ref _completionSource, newCompletionSource, null) is { } completionSource)
        {
            newCompletionSource = null;

            //阻塞后续获取
            await Task.Delay(Timeout.Infinite, _blockTokenSource.Token).ConfigureAwait(false);

            //不会运行到这里的
            throw new InvalidOperationException($"{GetType()} block connect running error");
        }
        _ = Task.Run(async () =>
        {
            try
            {
                var duplexPipeContext = await InternalConnectAsync(cancellationToken).ConfigureAwait(false);
                newCompletionSource.TrySetResult(duplexPipeContext);
            }
            catch (Exception ex)
            {
                newCompletionSource.TrySetException(ex);
            }
        }, cancellationToken);

        return await newCompletionSource.Task.ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual ValueTask DisposeAsync()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
        return default;
    }

    /// <inheritdoc/>
    public virtual ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        _blockTokenSource.SilenceRelease();
        return default;
    }

    #endregion Public 方法

    #region Protected 方法

    /// <summary>
    /// 连接
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task<IDuplexPipeContext<TInputter, TOutputter>> InternalConnectAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 对象已处置时抛出异常
    /// </summary>
    protected void ThrowIfDisposed()
    {
        ObjectDisposedExceptionHelper.ThrowIf(_isDisposed == 1, this);
    }

    #endregion Protected 方法

    #region IDisposable

    /// <summary>
    ///
    /// </summary>
    ~SingleConnectionDuplexPipeConnector()
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
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 1)
        {
            _blockTokenSource.Dispose();
        }
    }

    #endregion IDisposable
}
