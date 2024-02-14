using System.IO.Pipelines;
using Hoarwell.Enhancement.Buffers;

namespace Hoarwell.Outputters;

/// <summary>
/// 抽象的<inheritdoc cref="IOutputter"/>
/// </summary>
public abstract class Outputter : IOutputter
{
    #region Private 字段

    private bool _disposedValue;

    #endregion Private 字段

    #region Protected 字段

    /// <summary>
    /// 消息输出序列化委托
    /// </summary>
    protected readonly SerializeOutboundMessageDelegate SerializeOutboundMessageDelegate;

    #endregion Protected 字段

    #region Public 属性

    /// <summary>
    /// buffer初始化容量
    /// </summary>
    public int BufferInitialCapacity { get; }

    #endregion Public 属性

    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="Outputter"/>
    /// </summary>
    /// <param name="serializeOutboundMessageDelegate">使用的消息输出序列化委托</param>
    /// <param name="bufferInitialCapacity">buffer初始化容量</param>
    public Outputter(SerializeOutboundMessageDelegate serializeOutboundMessageDelegate, int bufferInitialCapacity)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(serializeOutboundMessageDelegate);

        SerializeOutboundMessageDelegate = serializeOutboundMessageDelegate;

        BufferInitialCapacity = bufferInitialCapacity;
    }

    #endregion Public 构造函数

    #region Static Create

    /// <summary>
    /// 使用 <paramref name="stream"/> 创建一个输出器
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="serializeOutboundMessageDelegate">使用的消息输出序列化委托</param>
    /// <param name="bufferInitialCapacity">buffer初始化容量</param>
    /// <returns></returns>
    public static Outputter Create(Stream stream, SerializeOutboundMessageDelegate serializeOutboundMessageDelegate, int bufferInitialCapacity)
    {
        return new StreamOutputter(stream, serializeOutboundMessageDelegate, bufferInitialCapacity);
    }

    /// <summary>
    /// 使用 <paramref name="writer"/> 创建一个输出器
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="serializeOutboundMessageDelegate">使用的消息输出序列化委托</param>
    /// <param name="bufferInitialCapacity">buffer初始化容量</param>
    /// <returns></returns>
    public static Outputter Create(PipeWriter writer, SerializeOutboundMessageDelegate serializeOutboundMessageDelegate, int bufferInitialCapacity)
    {
        return new PipeWriterOutputter(writer, serializeOutboundMessageDelegate, bufferInitialCapacity);
    }

    #endregion Static Create

    #region Public 方法

    /// <inheritdoc/>
    public abstract Task FlushAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual async Task WriteAndFlushAsync<T>(IHoarwellContext context, T message, CancellationToken cancellationToken = default)
    {
        var bufferWriter = CreateBufferWriter();
        try
        {
            var outboundMetadata = new OutboundMetadata(bufferWriter, message, typeof(T));

            await SerializeOutboundMessageDelegate(context, outboundMetadata).ConfigureAwait(false);

            await WriteAndFlushAsync(bufferWriter.WrittenMemory, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            DisposeBufferWriter(bufferWriter);
        }
    }

    /// <inheritdoc/>
    public abstract Task WriteAndFlushAsync(ReadOnlyMemory<byte> rawMessage, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual async Task WriteAsync<T>(IHoarwellContext context, T message, CancellationToken cancellationToken = default)
    {
        var bufferWriter = CreateBufferWriter();
        try
        {
            var outboundMetadata = new OutboundMetadata(bufferWriter, message, typeof(T));

            await SerializeOutboundMessageDelegate(context, outboundMetadata).ConfigureAwait(false);

            await WriteAsync(bufferWriter.WrittenMemory, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            DisposeBufferWriter(bufferWriter);
        }
    }

    /// <inheritdoc/>
    public abstract ValueTask WriteAsync(ReadOnlyMemory<byte> rawMessage, CancellationToken cancellationToken = default);

    #endregion Public 方法

    #region Protected 方法

    /// <summary>
    /// 创建 <see cref="IRandomAccessibleBufferWriter{T}"/>
    /// </summary>
    /// <returns></returns>
    protected virtual IRandomAccessibleBufferWriter<byte> CreateBufferWriter()
    {
        return new PooledByteRandomAccessibleBufferWriter(initialCapacity: BufferInitialCapacity);
    }

    /// <summary>
    /// 处置已使用结束的 <see cref="IRandomAccessibleBufferWriter{T}"/>
    /// </summary>
    /// <param name="bufferWriter"></param>
    protected virtual void DisposeBufferWriter(IRandomAccessibleBufferWriter<byte> bufferWriter)
    {
        (bufferWriter as IDisposable)?.Dispose();
    }

    /// <summary>
    /// 如果已处置则抛出异常
    /// </summary>
    protected void ThrowIfDisposed()
    {
        ObjectDisposedExceptionHelper.ThrowIf(_disposedValue, this);
    }

    #endregion Protected 方法

    #region Dispose

    /// <summary>
    ///
    /// </summary>
    ~Outputter()
    {
        Dispose(disposing: false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }
        _disposedValue = true;
    }

    #endregion Dispose
}
