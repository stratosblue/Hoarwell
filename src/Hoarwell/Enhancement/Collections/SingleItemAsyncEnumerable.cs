namespace Hoarwell.Enhancement.Collections;

/// <summary>
/// 单项元素 <inheritdoc cref="IAsyncEnumerable{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="item"></param>
public sealed class SingleItemAsyncEnumerable<T>(T item) : IAsyncEnumerable<T>
{
    #region Public 方法

    /// <inheritdoc/>
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new SingleItemAsyncEnumerator(item);

    #endregion Public 方法

    #region Internal 类

    internal sealed class SingleItemAsyncEnumerator(T item) : IAsyncEnumerator<T>
    {
        #region Private 字段

        private bool _canMove = true;

        #endregion Private 字段

        #region Public 属性

        /// <inheritdoc/>
        public T Current { get; } = item;

        #endregion Public 属性

        #region Public 方法

        /// <inheritdoc/>
        public ValueTask DisposeAsync() => default;

        /// <inheritdoc/>
        public ValueTask<bool> MoveNextAsync()
        {
            if (_canMove)
            {
                _canMove = false;
                return new ValueTask<bool>(true);
            }
            return new ValueTask<bool>(false);
        }

        #endregion Public 方法
    }

    #endregion Internal 类
}
