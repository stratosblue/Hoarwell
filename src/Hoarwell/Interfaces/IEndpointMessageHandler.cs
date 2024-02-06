namespace Hoarwell;

/// <summary>
/// 终结点消息处理器
/// </summary>
/// <typeparam name="T">消息类型</typeparam>
public interface IEndpointMessageHandler<in T>
{
    #region Public 方法

    /// <summary>
    /// 处理消息
    /// </summary>
    /// <param name="context"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    Task HandleAsync(IHoarwellContext context, T? input);

    #endregion Public 方法
}
