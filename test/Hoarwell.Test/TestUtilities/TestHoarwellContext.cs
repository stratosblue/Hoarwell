using Hoarwell.Features;

namespace Hoarwell.Test.TestUtilities;

internal class TestHoarwellContext : IHoarwellContext
{
    #region Public 属性

    public string ApplicationName { get; }

    public CancellationToken ExecutionAborted => ExecutionAbortedSource.Token;

    public CancellationTokenSource ExecutionAbortedSource { get; } = new();

    public IFeatureCollection Features { get; }

    public IOutputter Outputter { get; }

    public IDictionary<object, object> Properties { get; }

    public IServiceProvider Services { get; }

    #endregion Public 属性

    #region Public 构造函数

    public TestHoarwellContext(IFeatureCollection features, IOutputter outputter, IDictionary<object, object> properties, IServiceProvider serviceProvider)
    {
        ApplicationName = "Test";
        Features = features;
        Outputter = outputter;
        Properties = properties;
        Services = serviceProvider;
    }

    #endregion Public 构造函数

    #region Public 方法

    public void Abort()
    {
        ExecutionAbortedSource.Cancel();
    }

    public void Dispose()
    {
        ExecutionAbortedSource.Dispose();
    }

    #endregion Public 方法
}
