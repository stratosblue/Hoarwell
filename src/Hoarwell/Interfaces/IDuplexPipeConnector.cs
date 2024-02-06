using Microsoft.AspNetCore.Http.Features;

namespace Hoarwell;

/// <summary>
/// 双工管道连接器
/// </summary>
/// <typeparam name="TInputter">输入器类型</typeparam>
/// <typeparam name="TOutputter">输出器类型</typeparam>
public interface IDuplexPipeConnector<TInputter, TOutputter> : IAsyncDisposable
{
    #region Public 属性

    /// <summary>
    /// 特征集
    /// </summary>
    IFeatureCollection Features { get; }

    #endregion Public 属性

    #region Public 方法

    /// <summary>
    /// 连接并获取一个双工管道上下文
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<IDuplexPipeContext<TInputter, TOutputter>> ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止连接器
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask StopAsync(CancellationToken cancellationToken = default);

    #endregion Public 方法
}
