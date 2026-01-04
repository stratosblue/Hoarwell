using System.Collections;
using System.ComponentModel;

namespace Hoarwell.Features;

/// <summary>
/// 不可变的特征集合
/// </summary>
public sealed class ImmutableFeatureCollection : IFeatureCollection
{
    #region Private 字段

    private readonly Dictionary<Type, object?> _features;

    #endregion Private 字段

    #region Public 属性

    /// <summary>
    /// 空集合
    /// </summary>
    public static ImmutableFeatureCollection Empty { get; } = Builder().Build();

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    #endregion Public 属性

    #region Public 构造函数

    /// <inheritdoc cref="ImmutableFeatureCollection"/>
    public ImmutableFeatureCollection(IEnumerable<KeyValuePair<Type, object?>> features)
        : this(features.ToDictionary(m => m.Key, m => m.Value))
    {
    }

    #endregion Public 构造函数

    #region Private 构造函数

    /// <inheritdoc cref="ImmutableFeatureCollection"/>
    private ImmutableFeatureCollection(Dictionary<Type, object?> features)
    {
        _features = features ?? [];
    }

    #endregion Private 构造函数

    #region Public 方法

    /// <summary>
    /// 获取构造器
    /// </summary>
    /// <returns></returns>
    public static CollectionBuilder Builder(int capacity = 8)
    {
        return new(capacity);
    }

    /// <inheritdoc/>
    public TFeature? Get<TFeature>() where TFeature : class
    {
        return _features?.TryGetValue(typeof(TFeature), out var result) == true
               ? (TFeature?)result
               : default;
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<Type, object?>> GetEnumerator() => _features.GetEnumerator();

    /// <inheritdoc/>
    public bool Remove<TFeature>() where TFeature : class => throw new InvalidOperationException("Collection is readonly");

    /// <inheritdoc/>
    public void Set<TFeature>(TFeature? instance) where TFeature : class => throw new InvalidOperationException("Collection is readonly");

    /// <inheritdoc/>
    public bool TryGet<TFeature>(out TFeature? features) where TFeature : class
    {
        if (_features is null
            || !_features.TryGetValue(typeof(TFeature), out var result))
        {
            features = null;
            return false;
        }
        features = (TFeature?)result;
        return features != null;
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion Public 方法

    #region Public 类

    /// <summary>
    /// <see cref="ImmutableFeatureCollection"/> 构造器
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class CollectionBuilder(int capacity)
    {
        #region Private 字段

        private readonly Dictionary<Type, object?> _features = new(capacity);

        #endregion Private 字段

        #region Public 方法

        /// <summary>
        /// 添加特征
        /// </summary>
        /// <typeparam name="TFeature"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public CollectionBuilder Add<TFeature>(TFeature instance) where TFeature : class
        {
            _features.Add(typeof(TFeature), instance);
            return this;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <returns></returns>
        public ImmutableFeatureCollection Build()
        {
            return new(_features);
        }

        #endregion Public 方法
    }

    #endregion Public 类
}
