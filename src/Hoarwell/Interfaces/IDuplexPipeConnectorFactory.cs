namespace Hoarwell;

/// <summary>
/// 双工管道连接器工厂
/// </summary>
/// <typeparam name="TInputter">输入器类型</typeparam>
/// <typeparam name="TOutputter">输出器类型</typeparam>
public interface IDuplexPipeConnectorFactory<TInputter, TOutputter>
{
    #region Public 方法

    /// <summary>
    /// 获取连接器
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<IDuplexPipeConnector<TInputter, TOutputter>> GetAsync(CancellationToken cancellationToken = default);

    #endregion Public 方法
}
