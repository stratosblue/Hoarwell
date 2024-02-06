using Microsoft.Extensions.DependencyInjection;

namespace Hoarwell.Build;

/// <summary>
/// Hoarwell 构建器
/// </summary>
public class HoarwellBuilder
{
    #region Public 属性

    /// <summary>
    /// 应用程序名称，用于区分应用程序
    /// </summary>
    public string ApplicationName { get; }

    /// <summary>
    /// 服务集合
    /// </summary>
    public IServiceCollection Services { get; }

    #endregion Public 属性

    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="HoarwellBuilder"/>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="applicationName">应用程序名称</param>
    public HoarwellBuilder(IServiceCollection services, string applicationName)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(services);
        ArgumentNullExceptionHelper.ThrowIfNull(applicationName);

        Services = services;
        ApplicationName = applicationName;
    }

    #endregion Public 构造函数
}

/// <summary>
/// Hoarwell 构建器
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TInputter"></typeparam>
/// <typeparam name="TOutputter"></typeparam>
/// <param name="services"></param>
/// <param name="applicationName"></param>
public sealed class HoarwellBuilder<TContext, TInputter, TOutputter>(IServiceCollection services, string applicationName)
    : HoarwellBuilder(services, applicationName)
    where TContext : IHoarwellContext
{
    #region Public 构造函数

    /// <inheritdoc cref="HoarwellBuilder"/>
    public HoarwellBuilder(HoarwellBuilder builder) : this(builder.Services, builder.ApplicationName)
    {
    }

    #endregion Public 构造函数
}
