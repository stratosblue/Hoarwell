using Microsoft.Extensions.DependencyInjection;

namespace Hoarwell.Test.TestUtilities;

internal class TestHoarwellContextAccessorOptions
{
    #region Public 属性

    public Func<IServiceProvider, TestHoarwellContext> ContextCreateAction { get; set; } = (serviceProvider) =>
    {
        return (TestHoarwellContext)ActivatorUtilities.CreateInstance(serviceProvider, typeof(TestHoarwellContext))!;
    };

    #endregion Public 属性
}
