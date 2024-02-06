namespace Hoarwell;

/// <summary>
/// 应用程序运行程序
/// </summary>
public interface IHoarwellApplicationRunner : IAsyncDisposable
{
    #region Public 事件

    /// <summary>
    /// 上下文激活事件
    /// </summary>
    public event HoarwellContextActiveCallback? OnContextActive;

    /// <summary>
    /// 上下文失效事件
    /// </summary>
    public event HoarwellContextInactiveCallback? OnContextInactive;

    #endregion Public 事件

    #region Public 属性

    /// <summary>
    /// 应用程序停止 <see cref="CancellationToken"/>
    /// </summary>
    public CancellationToken ApplicationStopping { get; }

    #endregion Public 属性

    #region Public 方法

    /// <summary>
    /// 运行
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StopAsync(CancellationToken cancellationToken = default);

    #endregion Public 方法
}
