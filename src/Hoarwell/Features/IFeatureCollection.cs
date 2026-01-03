namespace Hoarwell.Features;

/// <summary>
/// 特征集合
/// </summary>
public interface IFeatureCollection : IEnumerable<KeyValuePair<Type, object?>>
{
    #region Public 方法

    /// <summary>
    /// 获取特征 <typeparamref name="Features"/>
    /// </summary>
    /// <typeparam name="Features"></typeparam>
    /// <returns></returns>
    Features? Get<Features>() where Features : class;

    /// <summary>
    /// 移除特征 <typeparamref name="Features"/>
    /// </summary>
    /// <typeparam name="Features"></typeparam>
    /// <returns></returns>
    bool Remove<Features>() where Features : class;

    /// <summary>
    /// 设置特征 <typeparamref name="Features"/>
    /// </summary>
    /// <typeparam name="Features"></typeparam>
    /// <param name="instance"></param>
    void Set<Features>(Features? instance) where Features : class;

    /// <summary>
    /// 获取特征 <typeparamref name="Features"/>
    /// </summary>
    /// <typeparam name="Features"></typeparam>
    /// <returns></returns>
    bool TryGet<Features>(out Features? features) where Features : class;

    #endregion Public 方法
}
