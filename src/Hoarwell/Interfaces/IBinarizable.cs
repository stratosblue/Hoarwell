using System.Buffers;

namespace Hoarwell;

/// <summary>
/// 可二进制化
/// </summary>
public interface IBinarizable
{
    #region Public 方法

    /// <summary>
    /// 将当前对象二进制化写入 <paramref name="bufferWriter"/>
    /// </summary>
    /// <param name="bufferWriter"></param>
    public void Serialize(in IBufferWriter<byte> bufferWriter);

    #endregion Public 方法
}
