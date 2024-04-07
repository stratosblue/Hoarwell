namespace Hoarwell.Test.TestUtilities;

internal class TestOutputter : IOutputter
{
    #region Public 方法

    public void Dispose()
    {
    }

    public Task FlushAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task WriteAndFlushAsync<T>(IHoarwellContext context, T message, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task WriteAndFlushAsync(ReadOnlyMemory<byte> rawMessage, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task WriteAsync<T>(IHoarwellContext context, T message, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public ValueTask WriteAsync(ReadOnlyMemory<byte> rawMessage, CancellationToken cancellationToken = default)
    {
        return default;
    }

    #endregion Public 方法
}
