namespace Hoarwell;

/// <summary>
/// 基于委托的 <see cref="IEndpointMessageHandler{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class DelegatingEndpointMessageHandler<T> : IEndpointMessageHandler<T>
{
    #region Private 字段

    private readonly MessageHandleDelegate<T> _handleDelegate;

    #endregion Private 字段

    #region Public 构造函数

    /// <inheritdoc cref="DelegatingEndpointMessageHandler{T}"/>
    public DelegatingEndpointMessageHandler(MessageHandleDelegate<T> handleDelegate)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(handleDelegate, nameof(handleDelegate));

        _handleDelegate = handleDelegate;
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public Task HandleAsync(IHoarwellContext context, T? input) => _handleDelegate(context, input);

    #endregion Public 方法
}
