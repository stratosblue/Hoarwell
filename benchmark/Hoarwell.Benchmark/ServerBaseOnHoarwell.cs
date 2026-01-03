using System.Net;
using Hoarwell.Benchmark.Hoarwell;
using Microsoft.Extensions.DependencyInjection;

namespace Hoarwell.Benchmark;

internal class ServerBaseOnHoarwell : IAsyncDisposable
{
    #region Public 字段

    public const int LengthDataSize = 2;

    #endregion Public 字段

    #region Private 字段

    private IHoarwellApplicationRunner _hoarwellApplicationRunner = null;

    private IServiceProvider _serviceProvider = null;

    #endregion Private 字段

    #region Public 构造函数

    public ServerBaseOnHoarwell()
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

    public async Task StartAsync(int port)
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddHoarwell("Server")
                .UseDefaultApplication()
                .UseAspNetCoreSocketTransportServer(options => options.EndPoints.Add(IPEndPoint.Parse($"127.0.0.1:{port}")))
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
                    pipelineBuilder.UseLengthFieldBasedFrameDecoder(new(LengthDataSize))
                                   .UseDefaultMessageDeserializer()
                                   .RunEndpoint(endpointBuilder =>
                                   {
                                       endpointBuilder.Handle<EchoData, HoarwellEchoDataServerMessageHandler>();
                                   });
                })
                .ConfigureOutboundPipeline(pipelineBuilder =>
                {
                    pipelineBuilder.UseLengthFieldBasedFrameEncoder(new(LengthDataSize))
                                   .RunDefaultMessageSerializer();
                });

        _serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });

        _hoarwellApplicationRunner = _serviceProvider.GetRequiredKeyedService<IHoarwellApplicationRunner>("Server");

        await _hoarwellApplicationRunner.StartAsync();
    }

    #endregion Public 方法
}
