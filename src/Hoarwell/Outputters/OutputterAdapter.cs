using System.IO.Pipelines;

namespace Hoarwell.Outputters;

/// <summary>
/// 输出器适配器
/// </summary>
public abstract class OutputterAdapter
{
    #region Public 字段

    /// <summary>
    /// 默认的 buffer 初始化容量
    /// </summary>
    public const int DefaultBufferInitialCapacity = 4 * 1024;

    #endregion Public 字段

    #region Public 方法

    /// <summary>
    /// 创建一个 <see cref="PipeWriter"/> 的输出器适配器
    /// </summary>
    /// <param name="bufferInitialCapacity"></param>
    /// <returns></returns>
    public static IOutputterAdapter<PipeWriter> CreatePipeWriterAdapter(int bufferInitialCapacity = DefaultBufferInitialCapacity)
    {
        return bufferInitialCapacity == DefaultBufferInitialCapacity
               ? PipeWriterOutputterAdapter.Shared
               : new PipeWriterOutputterAdapter(bufferInitialCapacity);
    }

    /// <summary>
    /// 创建一个 <see cref="Stream"/> 的输出器适配器
    /// </summary>
    /// <param name="bufferInitialCapacity"></param>
    /// <returns></returns>
    public static IOutputterAdapter<Stream> CreateStreamAdapter(int bufferInitialCapacity = DefaultBufferInitialCapacity)
    {
        return bufferInitialCapacity == DefaultBufferInitialCapacity
               ? StreamOutputterAdapter.Shared
               : new StreamOutputterAdapter(bufferInitialCapacity);
    }

    #endregion Public 方法
}

/// <summary>
/// 针对类型 <typeparamref name="T"/> 的输出器适配器
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="bufferInitialCapacity">buffer初始化容量</param>
public abstract class OutputterAdapter<T>(int bufferInitialCapacity)
    : OutputterAdapter, IOutputterAdapter<T>
{
    #region Public 属性

    /// <summary>
    /// buffer初始化容量
    /// </summary>
    public int BufferInitialCapacity { get; } = bufferInitialCapacity;

    #endregion Public 属性

    #region Public 方法

    /// <inheritdoc/>
    public abstract IOutputter Adapt(T outputter, SerializeOutboundMessageDelegate serializeOutboundMessageDelegate);

    #endregion Public 方法
}
