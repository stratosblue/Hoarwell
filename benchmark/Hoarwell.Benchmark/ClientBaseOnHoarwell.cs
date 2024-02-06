using System.Net;
using Hoarwell.Benchmark.Hoarwell;
using Hoarwell.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hoarwell.Benchmark;

internal class ClientBaseOnHoarwell
{
    #region Public 字段

    public const int LengthDataSize = 2;

    #endregion Public 字段

    #region Private 字段

    private IHoarwellContext _context;

    private EchoDataClientMessageHandler _handler = null;

    private IHoarwellApplicationRunner _hoarwellApplicationRunner = null;

    private IServiceProvider _serviceProvider = null;

    #endregion Private 字段

    #region Public 构造函数

    public ClientBaseOnHoarwell()
    {
    }

    #endregion Public 构造函数

    #region Public 方法

    public async ValueTask DisposeAsync()
    {
        await _hoarwellApplicationRunner.DisposeAsync();
        if (_serviceProvider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
    }

    public async Task StartAsync(int port, int echoCount)
    {
        var services = new ServiceCollection();
        services.AddLogging();

        _handler = new EchoDataClientMessageHandler(echoCount);
        services.TryAddKeyedSingleton("Client", _handler);

        services.AddHoarwell("Client")
                .UseDefaultApplication()
                .UseDefaultSocketTransportClient(options => options.EndPoints.Add(IPEndPoint.Parse($"127.0.0.1:{port}")))
                .UseDefaultSerializer(serializerBuilder =>
                {
                    serializerBuilder.AddMessage<EchoData>();
                })
                .UseDefaultTypeIdentifierAnalyzer(typeIdentifierAnalyzerBuilder =>
                {
                    typeIdentifierAnalyzerBuilder.AddMessage<EchoData>();
                })
                .ConfigureInboundPipeline(pipelineBuilder =>
                {
                    pipelineBuilder.UseLengthFieldBasedFrameDecoder(LengthDataSize)
                                   .UseDefaultMessageDeserializer()
                                   .RunEndpoint(endpointBuilder =>
                                   {
                                       endpointBuilder.Handle<EchoData, EchoDataClientMessageHandler>(_handler);
                                   });
                })
                .ConfigureOutboundPipeline(pipelineBuilder =>
                {
                    pipelineBuilder.UseLengthFieldBasedFrameEncoder(LengthDataSize)
                                   .RunDefaultMessageSerializer();
                });

        _serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });

        _hoarwellApplicationRunner = _serviceProvider.GetRequiredKeyedService<IHoarwellApplicationRunner>("Client");

        using var contextWaiter = _hoarwellApplicationRunner.GetContextWaiter();

        await _hoarwellApplicationRunner.StartAsync();

        _context = await contextWaiter.Task;
    }

    public async Task WaitCompleteAndResetAsync()
    {
        await _handler.TaskCompletionSource.Task;
        _handler.Reset();
    }

    public Task WriteAndFlushAsync<T>(T message)
    {
        return _context.WriteAndFlushAsync(message);
    }

    #endregion Public 方法
}
