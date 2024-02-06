using System.IO.Pipelines;

namespace Hoarwell.Outputters;

internal sealed class PipeWriterOutputterAdapter(int bufferInitialCapacity) : OutputterAdapter<PipeWriter>(bufferInitialCapacity)
{
    #region Public 属性

    public static PipeWriterOutputterAdapter Shared { get; } = new(DefaultBufferInitialCapacity);

    #endregion Public 属性

    #region Public 方法

    public override IOutputter Adapt(PipeWriter outputter, SerializeOutboundMessageDelegate serializeOutboundMessageDelegate)
    {
        return Outputter.Create(outputter, serializeOutboundMessageDelegate, BufferInitialCapacity);
    }

    #endregion Public 方法
}
