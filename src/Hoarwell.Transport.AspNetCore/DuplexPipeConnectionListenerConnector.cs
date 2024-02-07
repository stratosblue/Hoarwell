using System.IO.Pipelines;
using Hoarwell.Features;
using Hoarwell.Transport.AspNetCore.Features;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Features;

namespace Hoarwell.Transport.AspNetCore;

internal class DuplexPipeConnectionListenerConnector : IDuplexPipeConnector<PipeReader, PipeWriter>
{
    #region Private 字段

    private readonly IConnectionListener _connectionListener;

    #endregion Private 字段

    #region Public 属性

    public IFeatureCollection Features => throw new NotImplementedException();

    #endregion Public 属性

    #region Public 构造函数

    public DuplexPipeConnectionListenerConnector(IConnectionListener connectionListener)
    {
        _connectionListener = connectionListener ?? throw new ArgumentNullException(nameof(connectionListener));
    }

    #endregion Public 构造函数

    #region Public 方法

    public async ValueTask<IDuplexPipeContext<PipeReader, PipeWriter>> ConnectAsync(CancellationToken cancellationToken = default)
    {
        var connectionContext = await _connectionListener.AcceptAsync(cancellationToken).ConfigureAwait(false);

        if (connectionContext is not null)
        {
            var context = new DuplexPipeConnectionContextContext(connectionContext);

            context.Features.Set<IPipeEndPointFeature>(new PipeEndPointFeature(connectionContext.LocalEndPoint, connectionContext.RemoteEndPoint));

            return context;
        }
        throw new HoarwellException("Accept connection fail");
    }

    public ValueTask DisposeAsync()
    {
        return _connectionListener.DisposeAsync();
    }

    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        return _connectionListener.UnbindAsync(cancellationToken);
    }

    #endregion Public 方法
}
