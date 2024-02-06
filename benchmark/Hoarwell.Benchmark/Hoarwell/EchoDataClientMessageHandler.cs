namespace Hoarwell.Benchmark.Hoarwell;

internal class EchoDataClientMessageHandler : IEndpointMessageHandler<EchoData>
{
    #region Private 字段

    private readonly int _completeCount;

    private int _count;

    #endregion Private 字段

    #region Public 属性

    public TaskCompletionSource TaskCompletionSource { get; private set; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

    #endregion Public 属性

    #region Public 构造函数

    public EchoDataClientMessageHandler(int completeCount)
    {
        _completeCount = completeCount;
    }

    #endregion Public 构造函数

    #region Public 方法

    public Task HandleAsync(IHoarwellContext context, EchoData input)
    {
        if (Interlocked.Increment(ref _count) == _completeCount)
        {
            TaskCompletionSource.TrySetResult();
        }
        return Task.CompletedTask;
    }

    public void Reset()
    {
        _count = 0;
        TaskCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    #endregion Public 方法
}
