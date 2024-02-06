using System.Net;
using Chat.Shared;
using ChatRoom.Server;
using Hoarwell;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var endPoint = new IPEndPoint(IPAddress.Loopback, 12468);

var services = new ServiceCollection();

services.AddSingleton<ChatRoomImpl>();

services.AddHoarwell("Server")
        .UseDefaultApplication()
        .UseAspNetCoreSocketTransportServer(options => options.EndPoints.Add(endPoint))
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
            pipelineBuilder.UseUInt32LengthFieldBasedFrameDecoder()
                           .UseDefaultMessageDeserializer()
                           .RunEndpoint(endpointBuilder =>
                           {
                               endpointBuilder.Handle<ChatPacket, ChatPacketMessageHandler>();
                               endpointBuilder.Handle<ConnectPacket, ConnectPacketMessageHandler>();
                           });
        })
        .ConfigureOutboundPipeline(pipelineBuilder =>
        {
            pipelineBuilder.UseUInt32LengthFieldBasedFrameEncoder()
                           .RunDefaultMessageSerializer();
        });

services.AddLogging(builder => builder.AddSimpleConsole());

await using var serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });
await using var applicationRunner = serviceProvider.GetRequiredKeyedService<IHoarwellApplicationRunner>("Server");

await applicationRunner.StartAsync();

Console.WriteLine("press enter to exit.");
Console.ReadLine();
