namespace Hoarwell.Features;

/// <summary>
/// 空闲状态触发委托
/// </summary>
/// <param name="sender"></param>
/// <param name="context"></param>
/// <param name="state"></param>
/// <returns>返回true，则表明本次触发已处理，不需要下一步动作</returns>
public delegate ValueTask<bool> IdleStateTriggeredDelegate(IInboundOutboundIdleStateFeature sender, IHoarwellContext context, IdleState state);
