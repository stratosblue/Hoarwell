using System.IO.Pipelines;
using Hoarwell.Transport.AspNetCore.Features;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;

namespace Hoarwell.Transport.AspNetCore;

internal class DuplexPipeConnectionContextContext : IDuplexPipeContext<PipeReader, PipeWriter>
{
    #region Private 字段

    private readonly ConnectionContext _connectionContext;

    private int _disposed = 0;

    #endregion Private 字段

    #region Public 属性

    public Hoarwell.Features.IFeatureCollection Features { get; }

    public PipeReader Inputter { get; }

    public PipeWriter Outputter { get; }

    public CancellationToken PipeClosed { get; }

    #endregion Public 属性

    #region Public 构造函数

    public DuplexPipeConnectionContextContext(ConnectionContext connectionContext)
    {
        ArgumentNullException.ThrowIfNull(connectionContext);

        _connectionContext = connectionContext;

        Inputter = _connectionContext.Transport.Input;
        Outputter = _connectionContext.Transport.Output;
        PipeClosed = _connectionContext.ConnectionClosed;
        Features = new Hoarwell.Features.ConcurrentFeatureCollection(_connectionContext.Features!);

        Features.Set<Hoarwell.Features.IPipeLifetimeFeature>(new ConnectionPipeLifetimeFeature(_connectionContext.Features.GetRequiredFeature<IConnectionLifetimeFeature>()));
    }

    #endregion Public 构造函数

    #region Public 方法

    public void Abort()
    {
        _connectionContext.Abort();
    }

    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return default;
        }
        return _connectionContext.DisposeAsync();
    }

    #endregion Public 方法
}
