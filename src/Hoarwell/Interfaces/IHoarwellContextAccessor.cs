namespace Hoarwell;

/// <summary>
/// Hoarwell上下文访问器
/// </summary>
public interface IHoarwellContextAccessor
{
    #region Public 属性

    /// <summary>
    /// 上下文
    /// </summary>
    IHoarwellContext? Context { get; }

    #endregion Public 属性
}
