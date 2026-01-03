using System.Net;
using Hoarwell.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Hoarwell;

[TestClass]
public class CommunicationBaseTest
{
    #region Public 方法

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    [DataRow(6)]
    [DataRow(7)]
    [DataRow(8)]
    public async Task ShouldEchoSucceedAsync(int lengthDataSize)
    {
        var endpoint = IPEndPoint.Parse("127.0.0.1:12342");
        var serverRunnerInfo = await InitServer(endpoint, lengthDataSize);
        await serverRunnerInfo.Runner.StartAsync();

        var clientRunnerInfo = await InitClient(endpoint, lengthDataSize);
        using var waiter = clientRunnerInfo.Runner.GetContextWaiter();
        await clientRunnerInfo.Runner.StartAsync();

        try
        {
            var context = await waiter.Task;

            var handler = context.Services.GetRequiredService<EchoDataClientMessageHandler>();

            await context.WriteAndFlushAsync(new EchoData() { Id = 1, Message = "Hello" });

            await handler.TaskCompletionSource.Task;
        }
        finally
        {
            await DisposeRunnerInfoAsync(clientRunnerInfo);
            await DisposeRunnerInfoAsync(serverRunnerInfo);
        }
    }

    #endregion Public 方法

    #region Private 方法

    private async Task DisposeRunnerInfoAsync(RunnerInfo runnerInfo)
    {
        await runnerInfo.Runner.DisposeAsync();
        if (runnerInfo.ServiceProvider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
    }

    private Task<RunnerInfo> InitClient(EndPoint endPoint, int lengthDataSize)
    {
        var services = new ServiceCollection();

        var handler = new EchoDataClientMessageHandler(1);
        services.TryAddKeyedSingleton("Client", handler);
        services.TryAddSingleton(handler);

        services.AddHoarwell("Client")
                .UseDefaultApplication()
                .UseDefaultSocketTransportClient(options => options.EndPoints.Add(endPoint))
                .UseDefaultSerializer(serializerBuilder =>
                {
#if NET7_0_OR_GREATER
                    serializerBuilder.AddMessage<EchoData>();
#else
                    serializerBuilder.AddMessage(typeof(EchoData), ObjectBinaryParseHelper.WrapToParseAsObject<EchoData>(EchoData.TryParse));
#endif
                })
                .UseDefaultTypeIdentifierAnalyzer(typeIdentifierAnalyzerBuilder =>
                {
#if NET7_0_OR_GREATER
                    typeIdentifierAnalyzerBuilder.AddMessage<EchoData>();
#else
                    typeIdentifierAnalyzerBuilder.AddMessage(typeof(EchoData), EchoData.TypeIdentifier.ToArray());
#endif
                })
                .ConfigureInboundPipeline(pipelineBuilder =>
                {
                    pipelineBuilder.UseLengthFieldBasedFrameDecoder(new(lengthDataSize))
                                   .UseDefaultMessageDeserializer()
                                   .RunEndpoint(endpointBuilder =>
                                   {
                                       endpointBuilder.Handle<EchoData, EchoDataClientMessageHandler>(handler);
                                   });
                })
                .ConfigureOutboundPipeline(pipelineBuilder =>
                {
                    pipelineBuilder.UseLengthFieldBasedFrameEncoder(new(lengthDataSize))
                                   .RunDefaultMessageSerializer();
                });

        services.AddLogging(m => m.AddConsole());

        var serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });
        var applicationRunner = serviceProvider.GetRequiredKeyedService<IHoarwellApplicationRunner>("Client");

        var result = new RunnerInfo(serviceProvider, applicationRunner);
        return Task.FromResult(result);
    }

    private Task<RunnerInfo> InitServer(EndPoint endPoint, int lengthDataSize)
    {
        var services = new ServiceCollection();

        services.AddHoarwell("Server")
                .UseDefaultApplication()
                .UseAspNetCoreSocketTransportServer(options => options.EndPoints.Add(endPoint))
                .UseDefaultSerializer(serializerBuilder =>
                {
#if NET7_0_OR_GREATER
                    serializerBuilder.AddMessage<EchoData>();
#else
                    serializerBuilder.AddMessage(typeof(EchoData), ObjectBinaryParseHelper.WrapToParseAsObject<EchoData>(EchoData.TryParse));
#endif
                })
                .UseDefaultTypeIdentifierAnalyzer(typeIdentifierAnalyzerBuilder =>
                {
#if NET7_0_OR_GREATER
                    typeIdentifierAnalyzerBuilder.AddMessage<EchoData>();
#else
                    typeIdentifierAnalyzerBuilder.AddMessage(typeof(EchoData), EchoData.TypeIdentifier.ToArray());
#endif
                })
                .ConfigureInboundPipeline(pipelineBuilder =>
                {
                    pipelineBuilder.UseLengthFieldBasedFrameDecoder(new(lengthDataSize))
                                   .UseDefaultMessageDeserializer()
                                   .RunEndpoint(endpointBuilder =>
                                   {
                                       endpointBuilder.Handle<EchoData, HoarwellEchoDataServerMessageHandler>();
                                   });
                })
                .ConfigureOutboundPipeline(pipelineBuilder =>
                {
                    pipelineBuilder.UseLengthFieldBasedFrameEncoder(new(lengthDataSize))
                                   .RunDefaultMessageSerializer();
                });

        services.AddLogging(m => m.AddConsole());

        var serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });
        var applicationRunner = serviceProvider.GetRequiredKeyedService<IHoarwellApplicationRunner>("Server");

        var result = new RunnerInfo(serviceProvider, applicationRunner);
        return Task.FromResult(result);
    }

    #endregion Private 方法

    private record struct RunnerInfo(IServiceProvider ServiceProvider, IHoarwellApplicationRunner Runner);
}
