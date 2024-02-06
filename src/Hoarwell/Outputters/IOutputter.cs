namespace Hoarwell;

/// <summary>
/// 输出器
/// </summary>
public interface IOutputter : IDisposable
{
    #region Public 方法

    /// <summary>
    /// 将缓冲区写入目标
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task FlushAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用 <paramref name="context"/> 写入消息 <paramref name="message"/> 并将缓冲区写入目标
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task WriteAndFlushAsync<T>(IHoarwellContext context, T message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 写入消息 <paramref name="rawMessage"/> 并将缓冲区写入目标
    /// </summary>
    /// <param name="rawMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task WriteAndFlushAsync(ReadOnlyMemory<byte> rawMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用 <paramref name="context"/> 写入消息 <paramref name="message"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task WriteAsync<T>(IHoarwellContext context, T message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 写入消息 <paramref name="rawMessage"/>
    /// </summary>
    /// <param name="rawMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask WriteAsync(ReadOnlyMemory<byte> rawMessage, CancellationToken cancellationToken = default);

    #endregion Public 方法
}
