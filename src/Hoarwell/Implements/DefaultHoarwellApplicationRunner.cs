using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoarwell;

/// <summary>
/// 默认的 <inheritdoc cref="HoarwellApplicationRunner{TContext, TApplication, TInputter, TOutputter}"/>
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TApplication"></typeparam>
/// <typeparam name="TInputter"></typeparam>
/// <typeparam name="TOutputter"></typeparam>
public class DefaultHoarwellApplicationRunner<TContext, TApplication, TInputter, TOutputter>
    : HoarwellApplicationRunner<TContext, TApplication, TInputter, TOutputter>
    where TContext : IHoarwellContext
    where TApplication : IHoarwellApplication<TContext, TInputter, TOutputter>
{
    #region Public 构造函数

    /// <inheritdoc cref="DefaultHoarwellApplicationRunner{TContext, TApplication, TInputter, TOutputter}"/>
    public DefaultHoarwellApplicationRunner([ServiceKey] string applicationName,
                                            IServiceProvider serviceProvider,
                                            IServiceScopeFactory serviceScopeFactory,
                                            ILoggerFactory loggerFactory)
        : base(applicationName, serviceProvider, serviceScopeFactory, loggerFactory.CreateLogger("Hoarwell.DefaultHoarwellApplicationRunner"))
    {
    }

    #endregion Public 构造函数
}
