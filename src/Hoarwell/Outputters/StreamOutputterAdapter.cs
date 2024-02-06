namespace Hoarwell.Outputters;

internal sealed class StreamOutputterAdapter(int bufferInitialCapacity) : OutputterAdapter<Stream>(bufferInitialCapacity)
{
    #region Public 属性

    public static StreamOutputterAdapter Shared { get; } = new(DefaultBufferInitialCapacity);

    #endregion Public 属性

    #region Public 方法

    public override IOutputter Adapt(Stream outputter, SerializeOutboundMessageDelegate serializeOutboundMessageDelegate)
    {
        return Outputter.Create(outputter, serializeOutboundMessageDelegate, BufferInitialCapacity);
    }

    #endregion Public 方法
}
