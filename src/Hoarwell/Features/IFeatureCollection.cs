namespace Hoarwell.Features;

/// <summary>
/// 特征集合
/// </summary>
public interface IFeatureCollection : IEnumerable<KeyValuePair<Type, object?>>
{
    #region Public 属性

    /// <summary>
    /// 是否为只读集合
    /// </summary>
    public bool IsReadOnly { get; }

    #endregion Public 属性

    #region Public 方法

    /// <summary>
    /// 获取特征 <typeparamref name="TFeature"/>
    /// </summary>
    /// <typeparam name="TFeature"></typeparam>
    /// <returns></returns>
    TFeature? Get<TFeature>() where TFeature : class;

    /// <summary>
    /// 移除特征 <typeparamref name="TFeature"/>
    /// </summary>
    /// <typeparam name="TFeature"></typeparam>
    /// <returns></returns>
    bool Remove<TFeature>() where TFeature : class;

    /// <summary>
    /// 设置特征 <typeparamref name="TFeature"/>
    /// </summary>
    /// <typeparam name="TFeature"></typeparam>
    /// <param name="instance"></param>
    void Set<TFeature>(TFeature? instance) where TFeature : class;

    /// <summary>
    /// 获取特征 <typeparamref name="TFeature"/>
    /// </summary>
    /// <typeparam name="TFeature"></typeparam>
    /// <returns></returns>
    bool TryGet<TFeature>(out TFeature? features) where TFeature : class;

    #endregion Public 方法
}
