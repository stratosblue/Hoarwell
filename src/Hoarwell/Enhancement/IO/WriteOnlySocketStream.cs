using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace Hoarwell.Enhancement.IO;

/// <summary>
/// 只写的 <see cref="Socket"/> 流
/// </summary>
public class WriteOnlySocketStream : Stream
{
    #region Private 字段

    private readonly CancellationTokenSource _availableCTS;

    private readonly bool _ownsSocket;

    private readonly Socket _socket;

    private int _disposed = 0;

    private bool _writeable = true;

    #endregion Private 字段

    #region Public 属性

    /// <summary>
    /// 流可用 <see cref="CancellationToken"/>
    /// </summary>
    public CancellationToken AvailableCancellationToken { get; }

    /// <inheritdoc/>
    public override bool CanRead { get; } = false;

    /// <inheritdoc/>
    public override bool CanSeek { get; } = false;

    /// <inheritdoc/>
    public override bool CanTimeout { get; } = false;

    /// <inheritdoc/>
    public override bool CanWrite => _writeable;

    /// <inheritdoc/>
    public override long Length => throw NewNotSupportedException();

    /// <inheritdoc/>
    public override long Position { get => throw NewNotSupportedException(); set => throw NewNotSupportedException(); }

    /// <inheritdoc/>
    public override int ReadTimeout { get => throw NewNotSupportedException(); set => throw NewNotSupportedException(); }

    /// <inheritdoc/>
    public override int WriteTimeout { get => throw NewNotSupportedException(); set => throw NewNotSupportedException(); }

    #endregion Public 属性

    #region Public 构造函数

    /// <inheritdoc cref="WriteOnlySocketStream"/>
    public WriteOnlySocketStream(Socket socket, bool ownsSocket)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(socket);

        _socket = socket;
        _ownsSocket = ownsSocket;

        _availableCTS = new();
        AvailableCancellationToken = _availableCTS.Token;
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public override void Flush()
    {
    }

    /// <inheritdoc/>
    public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin) => throw NewNotSupportedException();

    /// <inheritdoc/>
    public override void SetLength(long value) => throw NewNotSupportedException();

    #region Read

    /// <inheritdoc/>
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) => throw NewNotSupportedException();

    /// <inheritdoc/>
    public override int EndRead(IAsyncResult asyncResult) => throw NewNotSupportedException();

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count) => throw NewNotSupportedException();

    /// <inheritdoc/>
    public override int Read(Span<byte> buffer) => throw NewNotSupportedException();

    /// <inheritdoc/>
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => throw NewNotSupportedException();

    /// <inheritdoc/>
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => throw NewNotSupportedException();

    /// <inheritdoc/>
    public override int ReadByte() => throw NewNotSupportedException();

    #endregion Read

    #region Write

    /// <inheritdoc/>
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        ThrowIfDisposed();

        try
        {
            return _socket.BeginSend(buffer,
                                     offset,
                                     count,
                                     SocketFlags.None,
                                     callback,
                                     state);
        }
        catch (Exception ex)
        {
            OnException(ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public override void EndWrite(IAsyncResult asyncResult)
    {
        ThrowIfDisposed();

        ArgumentNullExceptionHelper.ThrowIfNull(asyncResult);

        try
        {
            _socket.EndSend(asyncResult);
        }
        catch (Exception ex)
        {
            OnException(ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count) => Write(buffer.AsSpan(offset, count));

    /// <inheritdoc/>
    public override void Write(ReadOnlySpan<byte> buffer)
    {
        ThrowIfDisposed();

        try
        {
            _socket.Send(buffer, SocketFlags.None);
        }
        catch (Exception ex)
        {
            OnException(ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        var memory = buffer.AsMemory(offset, count);

        try
        {
            await _socket.SendAsync(memory, SocketFlags.None, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            OnException(ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        try
        {
            await _socket.SendAsync(buffer, SocketFlags.None, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            OnException(ex);
            throw;
        }
    }

    #endregion Write

    #endregion Public 方法

    #region Protected 方法

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        _writeable = false;

        _availableCTS.SilenceRelease();

        _availableCTS.Dispose();

        if (_ownsSocket)
        {
            try
            {
                _socket.Disconnect(true);
            }
            catch
            {
                //静默异常
            }
            _socket.Close();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// 新建NotSupported异常
    /// </summary>
    /// <param name="callerMemberName"></param>
    /// <returns></returns>
    protected Exception NewNotSupportedException([CallerMemberName] string? callerMemberName = null) => new StreamOperationNotSupportedException($"The operation \"{callerMemberName}\" is not supported at current stream");

    /// <summary>
    /// 在操作出现异常时触发
    /// </summary>
    /// <param name="exception"></param>
    protected virtual void OnException(Exception exception)
    {
        if (exception is not StreamOperationNotSupportedException)
        {
            Dispose();
        }
    }

    #endregion Protected 方法

    #region Private 方法

    private void ThrowIfDisposed() => ObjectDisposedExceptionHelper.ThrowIf(_disposed != 0, this);

    #endregion Private 方法
}
