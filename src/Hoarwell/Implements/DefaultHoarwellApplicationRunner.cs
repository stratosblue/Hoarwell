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

/// <summary>
/// 默认的 瞬态 <inheritdoc cref="HoarwellApplicationRunner{TContext, TApplication, TInputter, TOutputter}"/>
/// <br/>连接失败时会立即释放
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TApplication"></typeparam>
/// <typeparam name="TInputter"></typeparam>
/// <typeparam name="TOutputter"></typeparam>
public class DefaultTransientHoarwellApplicationRunner<TContext, TApplication, TInputter, TOutputter>
    : HoarwellApplicationRunner<TContext, TApplication, TInputter, TOutputter>
    where TContext : IHoarwellContext
    where TApplication : IHoarwellApplication<TContext, TInputter, TOutputter>
{
    #region Public 构造函数

    /// <inheritdoc cref="DefaultHoarwellApplicationRunner{TContext, TApplication, TInputter, TOutputter}"/>
    public DefaultTransientHoarwellApplicationRunner([ServiceKey] string applicationName,
                                                     IServiceProvider serviceProvider,
                                                     IServiceScopeFactory serviceScopeFactory,
                                                     ILoggerFactory loggerFactory)
        : base(applicationName, serviceProvider, serviceScopeFactory, loggerFactory.CreateLogger("Hoarwell.DefaultTransientHoarwellApplicationRunner"))
    {
    }

    #endregion Public 构造函数

    #region Protected 方法

    /// <inheritdoc/>
    protected override async Task OnConnectLoopErrorAsync(IDuplexPipeConnector<TInputter, TOutputter> connector, Exception exception)
    {
        await base.OnConnectLoopErrorAsync(connector, exception).ConfigureAwait(false);
        await DisposeAsync().ConfigureAwait(false);
    }

    #endregion Protected 方法
}
