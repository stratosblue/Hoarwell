namespace Hoarwell;

/// <summary>
/// 输出器适配器, 为类型为 <typeparamref name="TRealOutputter"/> 的真实输出器适配接口 <see cref="IOutputter"/>
/// </summary>
/// <typeparam name="TRealOutputter">真实输出器</typeparam>
public interface IOutputterAdapter<TRealOutputter>
{
    #region Public 方法

    /// <summary>
    /// 为真实输出器 <paramref name="outputter"/> 创建适配后的 <see cref="IOutputter"/>
    /// </summary>
    /// <param name="outputter"></param>
    /// <param name="serializeOutboundMessageDelegate"></param>
    /// <returns></returns>
    IOutputter Adapt(TRealOutputter outputter, SerializeOutboundMessageDelegate serializeOutboundMessageDelegate);

    #endregion Public 方法
}
