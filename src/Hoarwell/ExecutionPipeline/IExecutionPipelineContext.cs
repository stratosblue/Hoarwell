namespace Hoarwell.ExecutionPipeline;

/// <summary>
/// 执行管道上下文
/// </summary>
public interface IExecutionPipelineContext : IDisposable
{
    #region Public 属性

    /// <summary>
    /// 当前上下文的 <inheritdoc cref="CancellationToken"/>
    /// </summary>
    CancellationToken ExecutionAborted { get; }

    /// <summary>
    /// 上下文中存放的属性
    /// </summary>
    IDictionary<object, object> Properties { get; }

    /// <summary>
    /// 当前所使用的 <see cref="TaskScheduler"/>
    /// </summary>
    TaskScheduler? Scheduler => null;

    /// <summary>
    /// <inheritdoc cref="IServiceProvider"/>
    /// </summary>
    IServiceProvider Services { get; }

    #endregion Public 属性
}
