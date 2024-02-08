namespace ChatRoomOverHttp.Client.Transport;

internal class DelayInitStream : Stream
{
    #region Private 字段

    private readonly Task<Stream> _innerStreamTask;

    #endregion Private 字段

    #region Public 属性

    public override bool CanRead => InnerStream.CanRead;

    public override bool CanSeek => InnerStream.CanSeek;

    public override bool CanWrite => InnerStream.CanWrite;

    public override long Length => InnerStream.Length;

    public override long Position { get => InnerStream.Position; set => InnerStream.Position = value; }

    #endregion Public 属性

    #region Private 属性

    private Stream InnerStream => _innerStreamTask.Result;

    #endregion Private 属性

    #region Public 构造函数

    public DelayInitStream(Task<Stream> innerStreamTask)
    {
        _innerStreamTask = innerStreamTask;
    }

    #endregion Public 构造函数

    #region Public 方法

    public override void Flush()
    {
        InnerStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return InnerStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return InnerStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        InnerStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        InnerStream.Write(buffer, offset, count);
    }

    #endregion Public 方法
}
