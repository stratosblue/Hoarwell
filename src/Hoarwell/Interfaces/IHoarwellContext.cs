using Hoarwell.ExecutionPipeline;
using Hoarwell.Features;

namespace Hoarwell;

/// <summary>
/// Hoarwell上下文
/// </summary>
public interface IHoarwellContext : IExecutionPipelineContext
{
    #region Public 属性

    /// <summary>
    /// 应用程序名称
    /// </summary>
    string ApplicationName { get; }

    /// <summary>
    /// 当前上下文关闭的原因
    /// </summary>
    object? CloseReason { get; }

    /// <summary>
    /// 当前上下文正在中止的 <inheritdoc cref="CancellationToken"/>
    /// </summary>
    CancellationToken ExecutionAborting { get; }

    /// <summary>
    /// 特征集
    /// </summary>
    IFeatureCollection Features { get; }

    /// <summary>
    /// 当前上下文的 <inheritdoc cref="IOutputter"/>
    /// </summary>
    IOutputter Outputter { get; }

    #endregion Public 属性

    #region Public 方法

    /// <summary>
    /// 中止上下文
    /// </summary>
    /// <param name="reason">中止原因</param>
    void Abort(object? reason = null);

    #endregion Public 方法
}
