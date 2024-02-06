using System.Buffers;

namespace Hoarwell.Enhancement.Buffers;

/// <summary>
/// 可随机访问的 <see cref="IBufferWriter{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRandomAccessibleBufferWriter<T> : IBufferWriter<T>
{
    #region Public 属性

    /// <summary>
    /// 容量
    /// </summary>
    int Capacity { get; }

    /// <summary>
    /// 空闲容量
    /// </summary>
    int FreeCapacity { get; }

    /// <summary>
    /// 已写入大小
    /// </summary>
    int WrittenCount { get; }

    /// <summary>
    /// 已写入的数据
    /// </summary>
    ReadOnlyMemory<byte> WrittenMemory { get; }

    #endregion Public 属性

    #region Public 方法

    /// <summary>
    /// 获取指定位置的 <see cref="Memory{T}"/>
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="sizeHint"></param>
    /// <returns></returns>
    Memory<T> GetMemory(in int offset, in int sizeHint);

    /// <summary>
    /// 获取指定位置的 <see cref="Span{T}"/>
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="sizeHint"></param>
    /// <returns></returns>
    Span<T> GetSpan(in int offset, in int sizeHint);

    #endregion Public 方法
}
