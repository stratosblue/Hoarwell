using DotNetty.Transport.Channels;

namespace Hoarwell.Benchmark.DotNetty;

internal class DotNettyEchoDataClientMessageHandler : SimpleChannelInboundHandler<EchoData>
{
    #region Private 字段

    private readonly int _completeCount;

    private int _count;

    #endregion Private 字段

    #region Public 属性

    public TaskCompletionSource TaskCompletionSource { get; private set; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

    #endregion Public 属性

    #region Public 构造函数

    public DotNettyEchoDataClientMessageHandler(int completeCount)
    {
        _completeCount = completeCount;
    }

    #endregion Public 构造函数

    #region Protected 方法

    public void Reset()
    {
        _count = 0;
        TaskCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    protected override void ChannelRead0(IChannelHandlerContext ctx, EchoData msg)
    {
        if (Interlocked.Increment(ref _count) == _completeCount)
        {
            TaskCompletionSource.TrySetResult();
        }
    }

    #endregion Protected 方法
}
