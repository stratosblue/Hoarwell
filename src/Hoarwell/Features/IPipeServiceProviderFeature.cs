namespace Hoarwell.Features;

/// <summary>
/// <see cref="IServiceProvider"/> 特性
/// </summary>
public interface IServiceProviderFeature
{
    #region Public 属性

    /// <inheritdoc cref="IServiceProvider"/>
    IServiceProvider Services { get; }

    #endregion Public 属性
}
