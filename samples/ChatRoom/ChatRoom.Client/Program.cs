using System.Net;
using Chat.Shared;
using ChatRoom.Client;
using Hoarwell;
using Hoarwell.Extensions;
using Microsoft.Extensions.DependencyInjection;

var endPoint = new IPEndPoint(IPAddress.Loopback, 12468);

var services = new ServiceCollection();

services.AddHoarwell("Client")
        .UseDefaultStreamApplication()
#if OVER_HTTP
        .UseHttpBaseTransportClient(options => options.EndPoints.Add(new ChatRoomOverHttp.Client.Transport.HttpEndPoint($"http://{endPoint}/ChatRoom")))
#else
        .UseDefaultSocketTransportClient(options => options.EndPoints.Add(endPoint))
#endif
        .UseDefaultSerializer(serializerBuilder =>
        {
            serializerBuilder.AddMessage<ChatPacket>();
            serializerBuilder.AddMessage<ConnectPacket>();
        })
        .UseDefaultTypeIdentifierAnalyzer(typeIdentifierAnalyzerBuilder =>
        {
            typeIdentifierAnalyzerBuilder.AddMessage<ChatPacket>();
            typeIdentifierAnalyzerBuilder.AddMessage<ConnectPacket>();
        })
        .ConfigureInboundPipeline(pipelineBuilder =>
        {
            pipelineBuilder.UsePipeReaderAdaptMiddleware()
                           .UseUInt32LengthFieldBasedFrameDecoder()
                           .UseDefaultMessageDeserializer()
                           .RunEndpoint(endpointBuilder =>
                           {
                               endpointBuilder.Handle<ChatPacket, ChatPacketMessageHandler>(ServiceLifetime.Singleton);
                           });
        })
        .ConfigureOutboundPipeline(pipelineBuilder =>
        {
            pipelineBuilder.UseUInt32LengthFieldBasedFrameEncoder()
                           .RunDefaultMessageSerializer();
        });

services.AddLogging();

await using var serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });
await using var applicationRunner = serviceProvider.GetRequiredKeyedService<IHoarwellApplicationRunner>("Client");

using var contextWaiter = applicationRunner.GetContextWaiter();

await applicationRunner.StartAsync();

var context = await contextWaiter.Task;

context.ExecutionAborted.Register(() =>
{
    Console.WriteLine("your lose the connection");
    Environment.Exit(0);
});

Console.WriteLine("input your name:");
var name = Console.ReadLine()?.Trim() ?? "Guest";

await context.WriteAndFlushAsync(new ConnectPacket() { Name = name });

while (true)
{
    var message = Console.ReadLine();
    if (string.IsNullOrEmpty(message))
    {
        continue;
    }

    var packet = new ChatPacket()
    {
        Name = name,
        Message = message
    };
    await context.WriteAndFlushAsync(packet);
}
