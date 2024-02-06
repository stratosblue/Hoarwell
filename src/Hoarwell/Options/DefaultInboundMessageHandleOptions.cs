namespace Hoarwell.Options;

internal sealed class DefaultInboundMessageHandleOptions
{
    #region Public 属性

    public Dictionary<Type, HandleInboundMessageDelegate>? HandleInboundMessageDelegateMap { get; set; }

    #endregion Public 属性
}
