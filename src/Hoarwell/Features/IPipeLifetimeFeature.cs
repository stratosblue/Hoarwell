namespace Hoarwell.Features;

/// <summary>
/// 管道生命周期特性
/// </summary>
public interface IPipeLifetimeFeature
{
    #region Public 属性

    /// <summary>
    /// 管道关闭 <see cref="CancellationToken"/>
    /// </summary>
    CancellationToken PipeClosed { get; }

    /// <summary>
    /// 中止管道
    /// </summary>
    /// <returns></returns>
    void Abort();

    #endregion Public 属性
}
