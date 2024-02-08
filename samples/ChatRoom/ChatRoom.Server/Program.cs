using System.Net;
using Chat.Shared;
using ChatRoom.Server;
using Hoarwell;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#if OVER_HTTP

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.AspNetCore.Http.Timeouts;

#endif

var endPoint = new IPEndPoint(IPAddress.Loopback, 12468);

#if OVER_HTTP

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(options =>
{
    options.Listen(endPoint, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});
var services = builder.Services;

#else

var services = new ServiceCollection();

#endif

services.AddSingleton<ChatRoomImpl>();

services.AddHoarwell("Server")
        .UseDefaultStreamApplication()
#if OVER_HTTP
        .UseHttpBaseTransportServer()
#else
        .UseDefaultSocketTransportServer(options => options.EndPoints.Add(endPoint))
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

#if OVER_HTTP
await using var app = builder.Build();

app.MapPost("/ChatRoom", async (HttpContext context, ChatRoomOverHttp.Server.Transport.HttpBasePipeConnector connector) =>
{
    if (context.Features.Get<IHttpMinRequestBodyDataRateFeature>() is { } rateFeature)
    {
        rateFeature.MinDataRate = null;
    }
    if (context.Features.Get<IHttpMaxRequestBodySizeFeature>() is { } bodySizeFeature)
    {
        bodySizeFeature.MaxRequestBodySize = null;
    }
    if (context.Features.Get<IHttpRequestTimeoutFeature>() is { } timeoutFeature)
    {
        timeoutFeature.DisableTimeout();
    }

    context.Response.Headers.ContentType = "application/hoarwell";
    await context.Response.StartAsync();

    var cancellationToken = await connector.ConnectAsync(context);
    await Task.Delay(Timeout.Infinite, cancellationToken);
});
var serviceProvider = app.Services;

await app.StartAsync();
#else
await using var serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });
#endif

await using var applicationRunner = serviceProvider.GetRequiredKeyedService<IHoarwellApplicationRunner>("Server");

await applicationRunner.StartAsync();

Console.WriteLine("press enter to exit.");
Console.ReadLine();
