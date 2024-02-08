using System.Net;
using System.Net.Http.Headers;

namespace ChatRoomOverHttp.Client.Transport;

internal class LongAliveHttpContent : HttpContent
{
    #region Private 字段

    private readonly TaskCompletionSource<Stream> _streamGetCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

    private readonly TaskCompletionSource _taskCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

    #endregion Private 字段

    #region Public 构造函数

    public LongAliveHttpContent()
    {
        Headers.ContentType = new MediaTypeHeaderValue("application/hoarwell");
    }

    #endregion Public 构造函数

    #region Public 方法

    public void Complete()
    {
        _taskCompletionSource.TrySetResult();
    }

    public Task<Stream> GetStreamAsync()
    {
        return _streamGetCompletionSource.Task;
    }

    #endregion Public 方法

    #region Protected 方法

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        _streamGetCompletionSource.SetResult(stream);
        return _taskCompletionSource.Task;
    }

    protected override bool TryComputeLength(out long length)
    {
        length = -1;
        return false;
    }

    #endregion Protected 方法
}
