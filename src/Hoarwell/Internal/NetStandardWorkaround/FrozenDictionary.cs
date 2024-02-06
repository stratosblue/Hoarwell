using System.Runtime.Serialization;

namespace System.Collections.Frozen;

internal class FrozenDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    #region Public 构造函数

    public FrozenDictionary()
    {
    }

    public FrozenDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
    {
    }

    public FrozenDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection)
    {
    }

    public FrozenDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
    {
    }

    public FrozenDictionary(int capacity) : base(capacity)
    {
    }

    public FrozenDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
    {
    }

    public FrozenDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer) : base(collection, comparer)
    {
    }

    public FrozenDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
    {
    }

    #endregion Public 构造函数

    #region Protected 构造函数

    protected FrozenDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    #endregion Protected 构造函数
}

internal static class FrozenDictionary
{
    #region Public 方法

    public static FrozenDictionary<TKey, TValue> ToFrozenDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey>? comparer = null) where TKey : notnull
    {
        return comparer is null
               ? new FrozenDictionary<TKey, TValue>(source)
               : new FrozenDictionary<TKey, TValue>(source, comparer);
    }

    public static FrozenDictionary<TKey, TElement> ToFrozenDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer = null) where TKey : notnull
    {
        var dictionary = source.ToDictionary(keySelector, elementSelector, comparer);
        return comparer is null
               ? new FrozenDictionary<TKey, TElement>(dictionary)
               : new FrozenDictionary<TKey, TElement>(dictionary, comparer);
    }

    #endregion Public 方法
}
