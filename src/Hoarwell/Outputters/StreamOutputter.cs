namespace Hoarwell.Outputters;

internal sealed class StreamOutputter : Outputter
{
    #region Private 字段

    private readonly Stream _stream;

    #endregion Private 字段

    #region Public 构造函数

    public StreamOutputter(Stream stream, SerializeOutboundMessageDelegate serializeOutboundMessageDelegate, int bufferInitialCapacity)
        : base(serializeOutboundMessageDelegate, bufferInitialCapacity)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(stream);

        _stream = stream;
    }

    #endregion Public 构造函数

    #region Public 方法

    public override Task FlushAsync(CancellationToken cancellationToken = default) => _stream.FlushAsync(cancellationToken);

    public override async Task WriteAndFlushAsync(ReadOnlyMemory<byte> rawMessage, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        await _stream.WriteAsync(rawMessage, cancellationToken).ConfigureAwait(false);
        await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> rawMessage, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        return _stream.WriteAsync(rawMessage, cancellationToken);
    }

    #endregion Public 方法
}
