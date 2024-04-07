namespace Hoarwell;

/// <summary>
/// 默认的 <inheritdoc cref="IHoarwellContextAccessor"/>
/// </summary>
internal sealed class HoarwellContextAccessor : IHoarwellContextAccessor
{
    #region Public 属性

    /// <inheritdoc/>
    public IHoarwellContext? Context { get; set; }

    #endregion Public 属性
}
