using Hoarwell.Features;
using Microsoft.AspNetCore.Http.Features;

namespace Hoarwell;

/// <summary>
/// 双工管道上下文
/// </summary>
/// <typeparam name="TInputter">输入器类型</typeparam>
/// <typeparam name="TOutputter">输出器类型</typeparam>
public interface IDuplexPipeContext<out TInputter, out TOutputter>
    : IDuplexPipeFeature<TInputter, TOutputter>, IPipeLifetimeFeature, IAsyncDisposable
{
    #region Public 属性

    /// <summary>
    /// 特征集
    /// </summary>
    IFeatureCollection Features { get; }

    #endregion Public 属性
}
