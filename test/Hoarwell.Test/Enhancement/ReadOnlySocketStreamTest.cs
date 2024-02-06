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

        Assert.ThrowsException<StreamOperationNotSupportedException>(() => stream.Length);
        Assert.ThrowsException<StreamOperationNotSupportedException>(() => stream.Position);
        Assert.ThrowsException<StreamOperationNotSupportedException>(() => stream.Position = 0);
        Assert.ThrowsException<StreamOperationNotSupportedException>(() => stream.ReadTimeout);
        Assert.ThrowsException<StreamOperationNotSupportedException>(() => stream.ReadTimeout = 0);
        Assert.ThrowsException<StreamOperationNotSupportedException>(() => stream.WriteTimeout);
        Assert.ThrowsException<StreamOperationNotSupportedException>(() => stream.WriteTimeout = 0);
        Assert.ThrowsException<StreamOperationNotSupportedException>(() => stream.Write(buffer, 0, buffer.Length));
        Assert.ThrowsException<StreamOperationNotSupportedException>(() => stream.Write(buffer));
        Assert.ThrowsException<StreamOperationNotSupportedException>(() => stream.WriteByte(1));
        await Assert.ThrowsExceptionAsync<StreamOperationNotSupportedException>(async () => await stream.WriteAsync(buffer, 0, buffer.Length));
        await Assert.ThrowsExceptionAsync<StreamOperationNotSupportedException>(async () => await stream.WriteAsync(buffer, 0, buffer.Length, default));
        await Assert.ThrowsExceptionAsync<StreamOperationNotSupportedException>(async () => await stream.WriteAsync(buffer));
        Assert.ThrowsException<StreamOperationNotSupportedException>(() => stream.BeginWrite(buffer, 0, 1, null, null));
        Assert.ThrowsException<StreamOperationNotSupportedException>(() => stream.EndWrite(null!));
    }

    #endregion Public 方法
}
