using Microsoft.AspNetCore.Http.Features;

namespace Hoarwell.Client;

/// <summary>
/// 单例 <inheritdoc cref="IDuplexPipeConnector{TInputter, TOutputter}"/>, 仅进行一次连接
/// </summary>
/// <typeparam name="TInputter"></typeparam>
/// <typeparam name="TOutputter"></typeparam>
public abstract class SingletonDuplexPipeConnector<TInputter, TOutputter>
    : IDuplexPipeConnector<TInputter, TOutputter>, IDisposable
{
    #region Private 字段

    private readonly CancellationTokenSource _blockTokenSource = new();

    private TaskCompletionSource<IDuplexPipeContext<TInputter, TOutputter>>? _completionSource;

    private bool _isDisposed;

    #endregion Private 字段

    #region Public 属性

    /// <inheritdoc/>
    public virtual IFeatureCollection Features { get; } = new FeatureCollection();

    #endregion Public 属性

    #region Public 方法

    /// <inheritdoc/>
    public virtual async ValueTask<IDuplexPipeContext<TInputter, TOutputter>> ConnectAsync(CancellationToken cancellationToken = default)
    {
        var newCompletionSource = new TaskCompletionSource<IDuplexPipeContext<TInputter, TOutputter>>();
        if (Interlocked.CompareExchange(ref _completionSource, newCompletionSource, null) is { } completionSource)
        {
            newCompletionSource = null;

            //阻塞后续获取
            await Task.Delay(Timeout.Infinite, _blockTokenSource.Token).ConfigureAwait(false);

            //不会运行到这里的
            throw new InvalidOperationException($"{GetType()} block context running error");
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

    #endregion Protected 方法

    #region IDisposable

    /// <summary>
    ///
    /// </summary>
    ~SingletonDuplexPipeConnector()
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
        if (!_isDisposed)
        {
            _blockTokenSource.Dispose();
            _isDisposed = true;
        }
    }

    #endregion IDisposable
}
