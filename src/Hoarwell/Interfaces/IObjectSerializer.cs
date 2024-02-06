using System.Buffers;

namespace Hoarwell;

/// <summary>
/// 对象序列化器
/// </summary>
public interface IObjectSerializer
{
    #region Public 方法

    /// <summary>
    /// 从 <paramref name="data"/> 反序列化类型 <paramref name="type"/> 的对象
    /// </summary>
    /// <param name="type"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public object? Deserialize(Type type, ReadOnlySequence<byte> data);

    /// <summary>
    /// 将 <paramref name="value"/> 按类型 <paramref name="type"/> 序列化到 <paramref name="bufferWriter"/>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <param name="bufferWriter"></param>
    public void Serialize(Type type, object? value, IBufferWriter<byte> bufferWriter);

    #endregion Public 方法
}
