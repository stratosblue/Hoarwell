using System.IO.Pipelines;
using System.Net.Sockets;
using Hoarwell.Client;
using Hoarwell.Enhancement.IO;
using Hoarwell.Features;

namespace Hoarwell.Extensions;

/// <summary>
/// <see cref="Socket"/> 的 <see cref="IDuplexPipeContext{TInputter, TOutputter}"/> 帮助类
/// </summary>
public static class SocketDuplexPipeContextHelper
{
    #region Public 方法

    /// <summary>
    /// 使用已连接的 <paramref name="socket"/> 创建一个默认的基于 <see cref="PipeReader"/> <see cref="PipeWriter"/> 的上下文对象
    /// </summary>
    /// <param name="socket"></param>
    /// <returns></returns>
    public static IDuplexPipeContext<PipeReader, PipeWriter> CreateDefaultPipeContext(Socket socket)
    {
        CancellationTokenSource? cts = null;

        try
        {
            var readStream = new ReadOnlySocketStream(socket, false);
            var writeStream = new WriteOnlySocketStream(socket, false);
            var pipeReader = PipeReader.Create(readStream);
            var pipeWriter = PipeWriter.Create(writeStream);

            cts = CancellationTokenSource.CreateLinkedTokenSource(readStream.AvailableCancellationToken, writeStream.AvailableCancellationToken);

            var context = new SocketConnectionContext<PipeReader, PipeWriter>(socket: socket,
                                                                              inputter: pipeReader,
                                                                              outputter: pipeWriter,
                                                                              lifetimeFeature: new DelegatingPipeLifetimeFeature(() =>
                                                                              {
                                                                                  socket.Close();
                                                                                  cts.SilenceRelease();
                                                                              }, cts.Token),
                                                                              disposeCallback: cts.Dispose);

            context.Features.Set<IPipeEndPointFeature>(new PipeEndPointFeature(socket.LocalEndPoint!, socket.RemoteEndPoint!));

            return context;
        }
        catch
        {
            cts.SilenceRelease();
            socket.Close();
            throw;
        }
    }

    /// <summary>
    /// 使用已连接的 <paramref name="socket"/> 创建一个默认的基于 <see cref="ReadOnlySocketStream"/> <see cref="WriteOnlySocketStream"/> 的上下文对象
    /// </summary>
    /// <param name="socket"></param>
    /// <returns></returns>
    public static IDuplexPipeContext<ReadOnlySocketStream, WriteOnlySocketStream> CreateDefaultStreamContext(Socket socket)
    {
        CancellationTokenSource? cts = null;

        try
        {
            var readStream = new ReadOnlySocketStream(socket, false);
            var writeStream = new WriteOnlySocketStream(socket, false);

            cts = CancellationTokenSource.CreateLinkedTokenSource(readStream.AvailableCancellationToken, writeStream.AvailableCancellationToken);

            var context = new SocketConnectionContext<ReadOnlySocketStream, WriteOnlySocketStream>(socket: socket,
                                                                                                   inputter: readStream,
                                                                                                   outputter: writeStream,
                                                                                                   lifetimeFeature: new DelegatingPipeLifetimeFeature(() =>
                                                                                                   {
                                                                                                       socket.Close();
                                                                                                       cts.SilenceRelease();
                                                                                                   }, cts.Token),
                                                                                                   disposeCallback: cts.Dispose);

            context.Features.Set<IPipeEndPointFeature>(new PipeEndPointFeature(socket.LocalEndPoint!, socket.RemoteEndPoint!));

            return context;
        }
        catch
        {
            cts.SilenceRelease();
            socket.Close();
            throw;
        }
    }

    #endregion Public 方法
}
