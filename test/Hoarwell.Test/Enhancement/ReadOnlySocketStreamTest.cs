using System.Net.Sockets;
using Hoarwell.Enhancement.IO;

namespace Hoarwell.Test.Enhancement;

[TestClass]
public class ReadOnlySocketStreamTest
{
    //TODO 完善数据测试

    #region Public 方法

    [TestMethod]
    public async Task ShouldHasCorrectStatus()
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var stream = new ReadOnlySocketStream(socket, false);

        Assert.IsTrue(stream.CanRead);
        Assert.IsFalse(stream.CanSeek);
        Assert.IsFalse(stream.CanTimeout);
        Assert.IsFalse(stream.CanWrite);

        var buffer = new byte[] { 1, 2, 3, 4 };

        Assert.ThrowsExactly<StreamOperationNotSupportedException>(() => stream.Length);
        Assert.ThrowsExactly<StreamOperationNotSupportedException>(() => stream.Position);
        Assert.ThrowsExactly<StreamOperationNotSupportedException>(() => stream.Position = 0);
        Assert.ThrowsExactly<StreamOperationNotSupportedException>(() => stream.ReadTimeout);
        Assert.ThrowsExactly<StreamOperationNotSupportedException>(() => stream.ReadTimeout = 0);
        Assert.ThrowsExactly<StreamOperationNotSupportedException>(() => stream.WriteTimeout);
        Assert.ThrowsExactly<StreamOperationNotSupportedException>(() => stream.WriteTimeout = 0);
        Assert.ThrowsExactly<StreamOperationNotSupportedException>(() => stream.Write(buffer, 0, buffer.Length));
        Assert.ThrowsExactly<StreamOperationNotSupportedException>(() => stream.Write(buffer));
        Assert.ThrowsExactly<StreamOperationNotSupportedException>(() => stream.WriteByte(1));
        await Assert.ThrowsExactlyAsync<StreamOperationNotSupportedException>(async () => await stream.WriteAsync(buffer, 0, buffer.Length));
        await Assert.ThrowsExactlyAsync<StreamOperationNotSupportedException>(async () => await stream.WriteAsync(buffer, 0, buffer.Length, default));
        await Assert.ThrowsExactlyAsync<StreamOperationNotSupportedException>(async () => await stream.WriteAsync(buffer));
        Assert.ThrowsExactly<StreamOperationNotSupportedException>(() => stream.BeginWrite(buffer, 0, 1, null, null));
        Assert.ThrowsExactly<StreamOperationNotSupportedException>(() => stream.EndWrite(null!));
    }

    #endregion Public 方法
}
