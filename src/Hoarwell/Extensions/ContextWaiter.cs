using System.ComponentModel;

namespace Hoarwell.Extensions;

/// <summary>
/// 上下文等待者，从 <see cref="IHoarwellApplicationRunner"/> 中等待第一个激活上下文（通常需要在 <see cref="IHoarwellApplicationRunner.StartAsync(CancellationToken)"/> 之前获取）
/// </summary>
public sealed class ContextWaiter : IDisposable
{
    #region Private 字段

    private readonly IHoarwellApplicationRunner _applicationRunner;

    private readonly TaskCompletionSource<IHoarwellContext> _taskCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

    #endregion Private 字段

    #region Public 属性

    /// <summary>
    /// 上下文获取任务
    /// </summary>
    public Task<IHoarwellContext> Task => _taskCompletionSource.Task;

    #endregion Public 属性

    #region Public 构造函数

    /// <inheritdoc cref="ContextWaiter"/>
    public ContextWaiter(IHoarwellApplicationRunner applicationRunner)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(applicationRunner, nameof(applicationRunner));

        _applicationRunner = applicationRunner;
        _applicationRunner.OnContextActive += OnApplicationRunnerContextActive;
    }

    #endregion Public 构造函数

    #region Private 方法

    private void OnApplicationRunnerContextActive(IHoarwellContext context)
    {
        if (_isDisposed != 0)
        {
            return;
        }
        _taskCompletionSource.TrySetResult(context);
        Dispose();
    }

    #endregion Private 方法

    #region IDisposable

    private int _isDisposed = 0;

    /// <summary>
    ///
    /// </summary>
    ~ContextWaiter()
    {
        _applicationRunner.OnContextActive -= OnApplicationRunnerContextActive;
        _taskCompletionSource.TrySetCanceled();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
        {
            _applicationRunner.OnContextActive -= OnApplicationRunnerContextActive;
            _taskCompletionSource.TrySetCanceled();
            GC.SuppressFinalize(this);
        }
    }

    #endregion IDisposable
}

/// <summary>
///
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ContextWaiterExtensions
{
    #region Public 构造函数

    /// <summary>
    /// 获取一个上下文等待者，从 <paramref name="applicationRunner"/> 中等待第一个激活上下文（通常需要在 <see cref="IHoarwellApplicationRunner.StartAsync(CancellationToken)"/> 之前获取）
    /// </summary>
    /// <param name="applicationRunner"></param>
    /// <returns></returns>
    public static ContextWaiter GetContextWaiter(this IHoarwellApplicationRunner applicationRunner)
    {
        return new ContextWaiter(applicationRunner);
    }

    #endregion Public 构造函数
}
