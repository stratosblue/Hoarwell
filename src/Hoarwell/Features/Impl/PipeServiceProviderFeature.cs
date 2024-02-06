namespace Hoarwell.Features;

internal sealed class PipeServiceProviderFeature : IServiceProviderFeature
{
    #region Public 属性

    public IServiceProvider Services { get; }

    #endregion Public 属性

    #region Public 构造函数

    public PipeServiceProviderFeature(IServiceProvider services)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(services);
        Services = services;
    }

    #endregion Public 构造函数
}
