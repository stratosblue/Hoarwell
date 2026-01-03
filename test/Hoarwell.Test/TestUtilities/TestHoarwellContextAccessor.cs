using Hoarwell.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Hoarwell.Test.TestUtilities;

internal class TestHoarwellContextAccessor : IHoarwellContextAccessor
{
    #region Public 属性

    public IHoarwellContext? Context { get; }

    #endregion Public 属性

    #region Public 构造函数

    public TestHoarwellContextAccessor(IServiceProvider serviceProvider, IOptions<TestHoarwellContextAccessorOptions> options)
    {
        Context = options.Value.ContextCreateAction(serviceProvider);
    }

    #endregion Public 构造函数

    #region Public 方法

    public static void AddTestHoarwellContextAccessor(IServiceCollection services, string applicationName, Action<TestHoarwellContextAccessorOptions>? optionsSetup = null)
    {
        if (optionsSetup is null)
        {
            services.AddOptions<TestHoarwellContextAccessorOptions>();
        }
        else
        {
            services.AddOptions<TestHoarwellContextAccessorOptions>().Configure(optionsSetup);
        }
        services.TryAddScoped<IHoarwellContextAccessor, TestHoarwellContextAccessor>();

        services.TryAddScoped<IFeatureCollection, TestFeatureCollection>();
        services.TryAddScoped<IOutputter, TestOutputter>();
        services.TryAddScoped<IDictionary<object, object>, TestContextProperties>();
    }

    #endregion Public 方法
}
