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

    public DelegatingPipeLifetimeFeature(CancellationToken cancellationToken, Action abortCallback)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(abortCallback, nameof(abortCallback));

        PipeClosed = cancellationToken;
        _abortCallback = abortCallback;
    }

    #endregion Public 构造函数

    #region Public 方法

    public void Abort() => _abortCallback();

    #endregion Public 方法
}
