using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Hoarwell.Features;

//see https://github.com/dotnet/aspnetcore/blob/release/10.0/src/Extensions/Features/src/FeatureCollection.cs

/// <summary>
/// 并发特征集合
/// </summary>
public class ConcurrentFeatureCollection
    : IFeatureCollection
{
    #region Private 字段

    private static readonly KeyComparer s_featureKeyComparer = new();

    private readonly IFeatureCollection? _defaults;

    private ConcurrentDictionary<Type, object?>? _features;

    private SpinLock _spinLock = new(false);

    #endregion Private 字段

    #region Public 属性

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    #endregion Public 属性

    #region Public 构造函数

    /// <inheritdoc cref="ConcurrentFeatureCollection"/>
    public ConcurrentFeatureCollection()
    {
    }

    /// <summary>
    /// <inheritdoc cref="ConcurrentFeatureCollection"/>
    /// </summary>
    /// <param name="defaults">默认特征集合</param>
    public ConcurrentFeatureCollection(IEnumerable<KeyValuePair<Type, object?>> defaults)
    {
        _defaults = new ConcurrentFeatureCollection()
        {
            _features = new(defaults),
        };
    }

    /// <summary>
    /// <inheritdoc cref="ConcurrentFeatureCollection"/>
    /// </summary>
    /// <param name="defaults">默认特征集合</param>
    public ConcurrentFeatureCollection(IFeatureCollection defaults)
    {
        _defaults = defaults;
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public TFeature? Get<TFeature>() where TFeature : class
    {
        return _features?.TryGetValue(typeof(TFeature), out var result) == true
               ? (TFeature?)result
               : _defaults?.Get<TFeature>();
    }

    /// <inheritdoc/>
    public bool Remove<TFeature>() where TFeature : class
    {
        return _features?.TryRemove(typeof(TFeature), out _) == true;
    }

    /// <inheritdoc/>
    public void Set<TFeature>(TFeature? instance) where TFeature : class
    {
        EnsureFeatures()[typeof(TFeature)] = instance;
    }

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

    #region Enumerable

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<Type, object?>> GetEnumerator()
    {
        if (_features != null)
        {
            foreach (var pair in _features)
            {
                yield return pair;
            }
        }

        if (_defaults != null)
        {
            // Don't return features masked by the wrapper.
            foreach (var pair in _features == null ? _defaults : _defaults.Except(_features, s_featureKeyComparer))
            {
                yield return pair;
            }
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion Enumerable

    #endregion Public 方法

    #region Private 方法

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ConcurrentDictionary<Type, object?> EnsureFeatures()
    {
        if (_features == null)
        {
            var lockTaken = false;

            _spinLock.Enter(ref lockTaken);
            _features ??= new();

            if (lockTaken)
            {
                _spinLock.Exit(false);
            }
        }

        return _features;
    }

    #endregion Private 方法

    #region Private 类

    private sealed class KeyComparer : IEqualityComparer<KeyValuePair<Type, object?>>
    {
        #region Public 方法

        public bool Equals(KeyValuePair<Type, object?> x, KeyValuePair<Type, object?> y)
        {
            return x.Key.Equals(y.Key);
        }

        public int GetHashCode(KeyValuePair<Type, object?> obj)
        {
            return obj.Key.GetHashCode();
        }

        #endregion Public 方法
    }

    #endregion Private 类
}
