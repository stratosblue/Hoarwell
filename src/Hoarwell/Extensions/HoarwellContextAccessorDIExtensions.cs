using System.ComponentModel;
using Hoarwell;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <see cref="IHoarwellContextAccessor"/> 拓展方法集合
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class HoarwellContextAccessorDIExtensions
{
    #region Public 方法

    /// <summary>
    /// 添加 <see cref="IHoarwellContextAccessor"/> 以便在DI容器中获取 <see cref="IHoarwellContext"/>
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddHoarwellContextAccessor(this IServiceCollection services)
    {
        services.TryAddScoped<HoarwellContextAccessor>();
        services.TryAddScoped<IHoarwellContextAccessor>(serviceProvider => serviceProvider.GetRequiredService<HoarwellContextAccessor>());
        return services;
    }

    #endregion Public 方法
}
