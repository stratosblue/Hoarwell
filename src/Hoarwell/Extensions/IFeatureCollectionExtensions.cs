using System.ComponentModel;

namespace Hoarwell.Features;

/// <summary>
/// FeatureCollection 拓展方法集合
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class IFeatureCollectionExtensions
{
    #region Public 方法

    /// <summary>
    /// 从<paramref name="features"/>中获取指定的<typeparamref name="TFeature"/>, 获取失败则抛出异常
    /// </summary>
    /// <typeparam name="TFeature"></typeparam>
    /// <param name="features"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static TFeature RequiredFeature<TFeature>(this IFeatureCollection features) where TFeature : class
    {
        return features.Get<TFeature>() ?? throw new KeyNotFoundException($"Not found {typeof(TFeature)} in feature collection.");
    }

    #endregion Public 方法
}
