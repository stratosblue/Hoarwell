namespace Hoarwell.Features;

/// <summary>
/// 双工管道特征
/// </summary>
/// <typeparam name="TInputter"></typeparam>
/// <typeparam name="TOutputter"></typeparam>
public interface IDuplexPipeFeature<out TInputter, out TOutputter>
{
    #region Public 属性

    /// <summary>
    /// 输入器
    /// </summary>
    TInputter Inputter { get; }

    /// <summary>
    /// 输出器
    /// </summary>
    TOutputter Outputter { get; }

    #endregion Public 属性
}
