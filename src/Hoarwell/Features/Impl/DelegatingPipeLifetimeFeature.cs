namespace Hoarwell.Features;

internal sealed class DelegatingPipeLifetimeFeature : IPipeLifetimeFeature
{
    #region Private 字段

    private readonly Action _abortCallback;

    #endregion Private 字段

    #region Public 属性

    public CancellationToken PipeClosed { get; }

    #endregion Public 属性

    #region Public 构造函数

    public DelegatingPipeLifetimeFeature(Action abortCallback, CancellationToken cancellationToken)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(abortCallback, nameof(abortCallback));

        _abortCallback = abortCallback;
        PipeClosed = cancellationToken;
    }

    #endregion Public 构造函数

    #region Public 方法

    public void Abort() => _abortCallback();

    #endregion Public 方法
}
