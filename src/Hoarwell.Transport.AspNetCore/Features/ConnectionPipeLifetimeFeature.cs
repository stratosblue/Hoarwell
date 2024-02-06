using Hoarwell.Features;
using Microsoft.AspNetCore.Connections.Features;

namespace Hoarwell.Transport.AspNetCore.Features;

internal class ConnectionPipeLifetimeFeature : IPipeLifetimeFeature
{
    #region Private 字段

    private readonly IConnectionLifetimeFeature _connectionLifetimeFeature;

    #endregion Private 字段

    #region Public 属性

    public CancellationToken PipeClosed => _connectionLifetimeFeature.ConnectionClosed;

    #endregion Public 属性

    #region Public 构造函数

    public ConnectionPipeLifetimeFeature(IConnectionLifetimeFeature connectionLifetimeFeature)
    {
        _connectionLifetimeFeature = connectionLifetimeFeature ?? throw new ArgumentNullException(nameof(connectionLifetimeFeature));
    }

    #endregion Public 构造函数

    #region Public 方法

    public void Abort()
    {
        _connectionLifetimeFeature.Abort();
    }

    #endregion Public 方法
}
