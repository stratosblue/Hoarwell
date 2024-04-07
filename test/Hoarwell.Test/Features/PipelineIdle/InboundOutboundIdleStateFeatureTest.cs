using Hoarwell.Features;
using Hoarwell.Options.Features;
using Hoarwell.Test.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoarwell.Test.Features.PipelineIdle;

[TestClass]
public class InboundOutboundIdleStateFeatureTest
{
    #region Private 字段

    private const string ApplicationName = "Test";

    private const int TimeoutMilliseconds = 100;

    #endregion Private 字段

    #region Public 方法

    #region InboundIdle Only

    [TestMethod]
    public async Task ShouldInboundIdleTimeoutTrigger()
    {
        using var serviceProvider = InitializeTestSrviceProvider(options => options.InboundIdleTimeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds));
        using var serviceScope = serviceProvider.CreateScope();
        var inboundOutboundIdleStateFeature = serviceScope.ServiceProvider.GetRequiredKeyedService<InboundOutboundIdleStateFeature>(ApplicationName);

        Assert.IsFalse(inboundOutboundIdleStateFeature.IdleStateTriggered);
        Assert.IsNull(inboundOutboundIdleStateFeature.TriggeredIdleState);

        await Task.Delay((int)(TimeoutMilliseconds * 1.5));

        Assert.IsTrue(inboundOutboundIdleStateFeature.IdleStateTriggered);
        Assert.AreEqual(IdleState.InboundIdle, inboundOutboundIdleStateFeature.TriggeredIdleState);
    }

    [TestMethod]
    public async Task ShouldInboundIdleTimeoutTriggerHandled()
    {
        using var serviceProvider = InitializeTestSrviceProvider(options => options.InboundIdleTimeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds));
        using var serviceScope = serviceProvider.CreateScope();
        var inboundOutboundIdleStateFeature = serviceScope.ServiceProvider.GetRequiredKeyedService<InboundOutboundIdleStateFeature>(ApplicationName);

        var onIdleStateTriggeredCalled = false;

        inboundOutboundIdleStateFeature.OnIdleStateTriggered += (sender, context, state) =>
        {
            onIdleStateTriggeredCalled = true;
            Assert.AreEqual(IdleState.InboundIdle, state);
            return new ValueTask<bool>(true);
        };

        Assert.IsFalse(inboundOutboundIdleStateFeature.IdleStateTriggered);
        Assert.IsNull(inboundOutboundIdleStateFeature.TriggeredIdleState);
        Assert.IsFalse(onIdleStateTriggeredCalled);

        await Task.Delay((int)(TimeoutMilliseconds * 1.5));

        Assert.IsTrue(onIdleStateTriggeredCalled);
        Assert.IsFalse(inboundOutboundIdleStateFeature.IdleStateTriggered);
        Assert.IsNull(inboundOutboundIdleStateFeature.TriggeredIdleState);
    }

    [TestMethod]
    public async Task ShouldInboundIdleTimeoutWork()
    {
        using var serviceProvider = InitializeTestSrviceProvider(options => options.InboundIdleTimeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds));
        using var serviceScope = serviceProvider.CreateScope();
        var inboundOutboundIdleStateFeature = serviceScope.ServiceProvider.GetRequiredKeyedService<InboundOutboundIdleStateFeature>(ApplicationName);

        for (int i = 0; i < 10; i++)
        {
            Assert.IsFalse(inboundOutboundIdleStateFeature.IdleStateTriggered);
            Assert.IsNull(inboundOutboundIdleStateFeature.TriggeredIdleState);

            inboundOutboundIdleStateFeature.UpdateInboundTime();
            await Task.Delay(TimeoutMilliseconds / 2);
        }

        await Task.Delay(TimeoutMilliseconds);
        Assert.IsTrue(inboundOutboundIdleStateFeature.IdleStateTriggered);
        Assert.AreEqual(IdleState.InboundIdle, inboundOutboundIdleStateFeature.TriggeredIdleState);
    }

    #endregion InboundIdle Only

    #region InboundIdle In Mixed

    [TestMethod]
    public async Task ShouldMixedInboundIdleTimeoutTrigger()
    {
        using var serviceProvider = InitializeTestSrviceProvider(options =>
        {
            options.InboundIdleTimeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds);
            options.OutboundIdleTimeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds * 2);
        });
        using var serviceScope = serviceProvider.CreateScope();
        var inboundOutboundIdleStateFeature = serviceScope.ServiceProvider.GetRequiredKeyedService<InboundOutboundIdleStateFeature>(ApplicationName);

        Assert.IsFalse(inboundOutboundIdleStateFeature.IdleStateTriggered);
        Assert.IsNull(inboundOutboundIdleStateFeature.TriggeredIdleState);

        await Task.Delay((int)(TimeoutMilliseconds * 1.5));

        Assert.IsTrue(inboundOutboundIdleStateFeature.IdleStateTriggered);
        Assert.AreEqual(IdleState.InboundIdle, inboundOutboundIdleStateFeature.TriggeredIdleState);
    }

    [TestMethod]
    public async Task ShouldMixedInboundIdleTimeoutWork()
    {
        using var serviceProvider = InitializeTestSrviceProvider(options =>
        {
            options.InboundIdleTimeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds);
            options.OutboundIdleTimeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds * 2);
        });
        using var serviceScope = serviceProvider.CreateScope();
        var inboundOutboundIdleStateFeature = serviceScope.ServiceProvider.GetRequiredKeyedService<InboundOutboundIdleStateFeature>(ApplicationName);

        for (int i = 0; i < 10; i++)
        {
            Assert.IsFalse(inboundOutboundIdleStateFeature.IdleStateTriggered);
            Assert.IsNull(inboundOutboundIdleStateFeature.TriggeredIdleState);

            inboundOutboundIdleStateFeature.UpdateInboundTime();
            inboundOutboundIdleStateFeature.UpdateOutboundTime();
            await Task.Delay(TimeoutMilliseconds / 2);
        }

        await Task.Delay(TimeoutMilliseconds);
        Assert.IsTrue(inboundOutboundIdleStateFeature.IdleStateTriggered);
        Assert.AreEqual(IdleState.InboundIdle, inboundOutboundIdleStateFeature.TriggeredIdleState);
    }

    #endregion InboundIdle In Mixed

    #region OutboundIdle Only

    [TestMethod]
    public async Task ShouldOutboundIdleTimeoutTrigger()
    {
        using var serviceProvider = InitializeTestSrviceProvider(options => options.OutboundIdleTimeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds));
        using var serviceScope = serviceProvider.CreateScope();
        var inboundOutboundIdleStateFeature = serviceScope.ServiceProvider.GetRequiredKeyedService<InboundOutboundIdleStateFeature>(ApplicationName);

        Assert.IsFalse(inboundOutboundIdleStateFeature.IdleStateTriggered);
        Assert.IsNull(inboundOutboundIdleStateFeature.TriggeredIdleState);

        await Task.Delay((int)(TimeoutMilliseconds * 1.5));

        Assert.IsTrue(inboundOutboundIdleStateFeature.IdleStateTriggered);
        Assert.AreEqual(IdleState.OutboundIdle, inboundOutboundIdleStateFeature.TriggeredIdleState);
    }

    [TestMethod]
    public async Task ShouldOutboundIdleTimeoutTriggerHandled()
    {
        using var serviceProvider = InitializeTestSrviceProvider(options => options.OutboundIdleTimeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds));
        using var serviceScope = serviceProvider.CreateScope();
        var inboundOutboundIdleStateFeature = serviceScope.ServiceProvider.GetRequiredKeyedService<InboundOutboundIdleStateFeature>(ApplicationName);

        var onIdleStateTriggeredCalled = false;

        inboundOutboundIdleStateFeature.OnIdleStateTriggered += (sender, context, state) =>
        {
            onIdleStateTriggeredCalled = true;
            Assert.AreEqual(IdleState.OutboundIdle, state);
            return new ValueTask<bool>(true);
        };

        Assert.IsFalse(inboundOutboundIdleStateFeature.IdleStateTriggered);
        Assert.IsNull(inboundOutboundIdleStateFeature.TriggeredIdleState);
        Assert.IsFalse(onIdleStateTriggeredCalled);

        await Task.Delay((int)(TimeoutMilliseconds * 1.5));

        Assert.IsTrue(onIdleStateTriggeredCalled);
        Assert.IsFalse(inboundOutboundIdleStateFeature.IdleStateTriggered);
        Assert.IsNull(inboundOutboundIdleStateFeature.TriggeredIdleState);
    }

    [TestMethod]
    public async Task ShouldOutboundIdleTimeoutWork()
    {
        using var serviceProvider = InitializeTestSrviceProvider(options => options.OutboundIdleTimeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds));
        using var serviceScope = serviceProvider.CreateScope();
        var inboundOutboundIdleStateFeature = serviceScope.ServiceProvider.GetRequiredKeyedService<InboundOutboundIdleStateFeature>(ApplicationName);

        for (int i = 0; i < 10; i++)
        {
            Assert.IsFalse(inboundOutboundIdleStateFeature.IdleStateTriggered);
            Assert.IsNull(inboundOutboundIdleStateFeature.TriggeredIdleState);

            inboundOutboundIdleStateFeature.UpdateOutboundTime();
            await Task.Delay(TimeoutMilliseconds / 2);
        }

        await Task.Delay(TimeoutMilliseconds);
        Assert.IsTrue(inboundOutboundIdleStateFeature.IdleStateTriggered);
        Assert.AreEqual(IdleState.OutboundIdle, inboundOutboundIdleStateFeature.TriggeredIdleState);
    }

    #endregion OutboundIdle Only

    #region OutboundIdle In Mixed

    [TestMethod]
    public async Task ShouldMixedOutboundIdleTimeoutTrigger()
    {
        using var serviceProvider = InitializeTestSrviceProvider(options =>
        {
            options.InboundIdleTimeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds * 2);
            options.OutboundIdleTimeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds);
        });
        using var serviceScope = serviceProvider.CreateScope();
        var inboundOutboundIdleStateFeature = serviceScope.ServiceProvider.GetRequiredKeyedService<InboundOutboundIdleStateFeature>(ApplicationName);

        Assert.IsFalse(inboundOutboundIdleStateFeature.IdleStateTriggered);
        Assert.IsNull(inboundOutboundIdleStateFeature.TriggeredIdleState);

        await Task.Delay((int)(TimeoutMilliseconds * 1.5));

        Assert.IsTrue(inboundOutboundIdleStateFeature.IdleStateTriggered);
        Assert.AreEqual(IdleState.OutboundIdle, inboundOutboundIdleStateFeature.TriggeredIdleState);
    }

    [TestMethod]
    public async Task ShouldMixedOutboundIdleTimeoutWork()
    {
        using var serviceProvider = InitializeTestSrviceProvider(options =>
        {
            options.InboundIdleTimeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds * 2);
            options.OutboundIdleTimeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds);
        });
        using var serviceScope = serviceProvider.CreateScope();
        var inboundOutboundIdleStateFeature = serviceScope.ServiceProvider.GetRequiredKeyedService<InboundOutboundIdleStateFeature>(ApplicationName);

        for (int i = 0; i < 10; i++)
        {
            Assert.IsFalse(inboundOutboundIdleStateFeature.IdleStateTriggered);
            Assert.IsNull(inboundOutboundIdleStateFeature.TriggeredIdleState);

            inboundOutboundIdleStateFeature.UpdateInboundTime();
            inboundOutboundIdleStateFeature.UpdateOutboundTime();
            await Task.Delay(TimeoutMilliseconds / 2);
        }

        await Task.Delay(TimeoutMilliseconds);
        Assert.IsTrue(inboundOutboundIdleStateFeature.IdleStateTriggered);
        Assert.AreEqual(IdleState.OutboundIdle, inboundOutboundIdleStateFeature.TriggeredIdleState);
    }

    #endregion OutboundIdle In Mixed

    #endregion Public 方法

    #region Protected 方法

    protected ServiceProvider InitializeTestSrviceProvider(Action<InboundOutboundIdleOptions> optionsSetup)
    {
        var services = new ServiceCollection();

        services.AddLogging(builder => builder.AddSimpleConsole());
        services.AddOptions<InboundOutboundIdleOptions>(ApplicationName).Configure(optionsSetup);
        TestHoarwellContextAccessor.AddTestHoarwellContextAccessor(services, ApplicationName);
        services.AddKeyedScoped<InboundOutboundIdleStateFeature, InboundOutboundIdleStateFeature>(ApplicationName);

        return services.BuildServiceProvider(true);
    }

    #endregion Protected 方法
}
