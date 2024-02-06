namespace Hoarwell.ExecutionPipeline;

/// <summary>
/// 抽象的 <see cref="IExecutionPipelineContext"/> 基类
/// </summary>
/// <param name="services"></param>
/// <param name="cancellationToken"></param>
public abstract class ExecutionPipelineContext(IServiceProvider services, CancellationToken cancellationToken) : IExecutionPipelineContext
{
    #region Private 字段

    private bool _disposedValue;

    #endregion Private 字段

    #region Public 属性

    /// <inheritdoc/>
    public CancellationToken ExecutionAborted { get; } = cancellationToken;

    /// <inheritdoc/>
    public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

    /// <inheritdoc/>
    public TaskScheduler? Scheduler { get; protected set; }

    /// <inheritdoc/>
    public IServiceProvider Services { get; } = services ?? throw new ArgumentNullException(nameof(services));

    #endregion Public 属性

    #region Protected 方法

    /// <summary>
    /// 如果当前对象已调用 <see cref="Dispose()"/> 则抛出异常
    /// </summary>
    protected void ThrowIfDisposed()
    {
        ObjectDisposedExceptionHelper.ThrowIf(_disposedValue, this);
    }

    #endregion Protected 方法

    #region IDisposable

    /// <summary>
    ///
    /// </summary>
    ~ExecutionPipelineContext()
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
        if (!_disposedValue)
        {
            _disposedValue = true;
        }
    }

    #endregion IDisposable
}
