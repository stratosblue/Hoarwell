using System.Net.Sockets;
using Hoarwell.Features;
using Microsoft.AspNetCore.Http.Features;

namespace Hoarwell.Client;

internal class SocketConnectionContext<TReader, TWriter> : IDuplexPipeContext<TReader, TWriter>
{
    #region Protected 字段

    protected readonly Socket _socket;

    #endregion Protected 字段

    #region Private 字段

    private readonly Action? _disposeCallback;

    private int _disposed = 0;

    #endregion Private 字段

    #region Public 属性

    public IFeatureCollection Features { get; }

    public TReader Inputter { get; }

    public TWriter Outputter { get; }

    public CancellationToken PipeClosed { get; }

    #endregion Public 属性

    #region Public 构造函数

    public SocketConnectionContext(Socket socket,
                                   TReader inputter,
                                   TWriter outputter,
                                   IPipeLifetimeFeature lifetimeFeature,
                                   Action? disposeCallback = null)
    {
        _socket = socket;
        Inputter = inputter;
        Outputter = outputter;

        _disposeCallback = disposeCallback;

        Features = new FeatureCollection(2);
        Features.Set<IPipeLifetimeFeature>(lifetimeFeature);

        PipeClosed = lifetimeFeature.PipeClosed;
    }

    #endregion Public 构造函数

    #region Public 方法

    public void Abort()
    {
        _socket.Close();
    }

    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return default;
        }

        try
        {
            _disposeCallback?.Invoke();
        }
        catch
        {
            //静默异常
        }
        try
        {
            _socket.Close();
        }
        catch
        {
            //静默异常
        }
        return default;
    }

    #endregion Public 方法
}
