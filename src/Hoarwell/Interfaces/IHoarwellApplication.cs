using Microsoft.AspNetCore.Http.Features;

namespace Hoarwell;

/// <summary>
/// Hoarwell应用程序
/// </summary>
/// <typeparam name="TContext">上下文类型</typeparam>
/// <typeparam name="TInputter">输入器类型</typeparam>
/// <typeparam name="TOutputter">输出器类型</typeparam>
public interface IHoarwellApplication<TContext, TInputter, TOutputter>
    where TContext : IHoarwellContext
{
    #region Public 方法

    /// <summary>
    /// 创建上下文
    /// </summary>
    /// <param name="features"></param>
    /// <returns></returns>
    ValueTask<TContext> CreateContext(IFeatureCollection features);

    /// <summary>
    /// 销毁上下文
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    ValueTask DisposeContext(TContext context);

    /// <summary>
    /// 执行应用程序处理
    /// </summary>
    /// <param name="context"></param>
    /// <param name="inputter"></param>
    /// <param name="outputter"></param>
    /// <returns></returns>
    Task ExecuteAsync(TContext context, TInputter inputter, TOutputter outputter);

    #endregion Public 方法
}
