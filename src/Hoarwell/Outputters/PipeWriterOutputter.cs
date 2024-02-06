using System.IO.Pipelines;

namespace Hoarwell.Outputters;

internal sealed class PipeWriterOutputter : Outputter
{
    #region Private 字段

    private readonly PipeWriter _pipeWriter;

    //TODO Use channel replace semaphore?
    private readonly SemaphoreSlim _writeSemaphore = new(1);

    #endregion Private 字段

    #region Public 构造函数

    public PipeWriterOutputter(PipeWriter pipeWriter, SerializeOutboundMessageDelegate serializeOutboundMessageDelegate, int bufferInitialCapacity)
        : base(serializeOutboundMessageDelegate, bufferInitialCapacity)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(pipeWriter);

        _pipeWriter = pipeWriter;
    }

    #endregion Public 构造函数

    #region Public 方法

    public override async Task FlushAsync(CancellationToken cancellationToken = default)
    {
        await _writeSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await _pipeWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _writeSemaphore.Release();
        }
    }

    public override async Task WriteAndFlushAsync(ReadOnlyMemory<byte> rawMessage, CancellationToken cancellationToken = default)
    {
        await _writeSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await _pipeWriter.WriteAsync(rawMessage, cancellationToken).ConfigureAwait(false);
            await _pipeWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _writeSemaphore.Release();
        }
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> rawMessage, CancellationToken cancellationToken = default)
    {
        await _writeSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await _pipeWriter.WriteAsync(rawMessage, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _writeSemaphore.Release();
        }
    }

    #endregion Public 方法

    #region Protected 方法

    protected override void Dispose(bool disposing)
    {
        _writeSemaphore.Dispose();

        base.Dispose(disposing);
    }

    #endregion Protected 方法
}
