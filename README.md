# Hoarwell

a network application framework for rapid development of maintainable protocol servers and clients. 一个用于快速开发可维护的协议的服务器和客户端的网络应用程序框架。

## 1. Intro

基于抽象输入/输出实现的数据处理/通讯框架，旨在方便的在 `Socket` 等底层协议上进行基于二进制数据协议的通信及业务开发。

### Features

- 框架基于抽象的`Inputter`、`Outputter`，可以运行在任意可抽象的传输层之上，而不局限于`Socket`，例如：`NamedPipe`、`Http`等；
- 基于 `管道` 的处理模型，可灵活组装、拓展；
- `处理管道` 借鉴了部分 `Netty`/`DotNetty`，但与其不同：
    - 入站(`Inbound`)与出站(`Outbound`)管道已拆分配置，不在混淆在一起配置；
    - 入站(`Inbound`)管道实现为强类型流转，而非 `object`，数据流转更明确；
- 框架当前只提供了部分保证基础运行的实现，如：帧头长度编码、解码等，未提供其它高层数据协议，如：`Http`、`gzip`等；
- 框架依赖于 `DependencyInjection`，能更好的与相关生态集成；
- 目标框架 `netstandard2.1`、`net8.0`；
- 支持 `NativeAOT`；

## 2. Design

### 2.1 [名词列表](./docs/nouns.md)

### 2.2 架构图

![架构图](./docs/assets/architecture_diagram.svg)

## 3. 如何使用

### 3.1 Note：
 - 入站出站管道需要分别配置，总共需要配置四次，分别为 `服务端入站`、`服务端出站`、`客户端入站`、`客户端出站`，且编码解码管道顺序应对应，即：
    - `客户端出站`编码管道对应`服务端入站`解码管道
    - `客户端入站`解码管道对应`服务端出站`编码管道
 - 框架已提供保障基础运行的默认组件，配置过程中的各个组件都可以自行实现来替代默认实现

### 3.2 消息及消息处理器
```C#
//消息，使用默认的序列化器时需要实现接口：IBinaryParseable<T>, IBinarizable, ITypeIdentifierProvider
public class Message : IBinaryParseable<Message>, IBinarizable, ITypeIdentifierProvider
{
    public static ReadOnlySpan<byte> TypeIdentifier => //TODO 返回标识符数据例如 BitConverter.GetBytes(10001)

    public static bool TryParse(in ReadOnlySequence<byte> input, [MaybeNullWhen(false)] out Message result)
    {
        //TODO 实现反序列化逻辑
    }

    public void Serialize(in IBufferWriter<byte> bufferWriter)
    {
        //TODO 实现序列化逻辑
    }
}

//消息处理器，需要实现接口：IEndpointMessageHandler<T>
public class MessageHandler : IEndpointMessageHandler<Message>
{
    public Task HandleAsync(IHoarwellContext context, Message? input)
    {
        //TODO 实现消息对应的业务逻辑
    }
}
```

-------

### 3.3 服务端
```C#
services.AddHoarwell("Server")  //添加名为 Server 的应用程序
        .UseDefaultApplication()    //使用默认应用程序
        .UseAspNetCoreSocketTransportServer(options => options.EndPoints.Add(endPoint)) //使用AspNetCore的SocketTransportFactory服务端传输，并配置监听地址为 endPoint（注意此处与客户端不同）
        .UseDefaultSerializer(serializerBuilder =>  //使用默认的对象序列化器
        {
            //添加支持的序列化的消息类型
            //TMessage 需要实现 IBinaryParseable<TMessage> 和 IBinarizable 接口
            //serializerBuilder.AddMessage<TMessage>();
        })
        .UseDefaultTypeIdentifierAnalyzer(typeIdentifierAnalyzerBuilder =>  //使用默认的类型标识符分析器
        {
            //添加支持的消息类型
            //TMessage 需要实现 ITypeIdentifierProvider 接口
            //typeIdentifierAnalyzerBuilder.AddMessage<TMessage>();
        })
        .ConfigureInboundPipeline(pipelineBuilder =>    //配置入站管道
        {
            //配置入站处理管道
            pipelineBuilder.UseUInt32LengthFieldBasedFrameDecoder() //使用基于 uint 的长度帧头解码器中间件
                           .UseDefaultMessageDeserializer() //使用默认的消息序列化中间件
                           .RunEndpoint(endpointBuilder =>  //配置入站终结点
                           {
                                //添加消息的处理器
                                //TMessage 需要已在 Serializer 配置支持
                                //TMessageHandler 需要实现 IEndpointMessageHandler<TMessage> 接口
                                //endpointBuilder.Handle<TMessage, TMessageHandler>();
                           });
        })
        .ConfigureOutboundPipeline(pipelineBuilder =>   //配置出站管道
        {
            //配置出站处理管道
            pipelineBuilder.UseUInt32LengthFieldBasedFrameEncoder() //使用基于 uint 的长度帧头解码器中间件
                           .RunDefaultMessageSerializer();  //运行默认的消息序列化出站终结点
        });

await using var serviceProvider = services.BuildServiceProvider();
await using var applicationRunner = serviceProvider.GetRequiredKeyedService<IHoarwellApplicationRunner>("Server");  //从 DI 容器中获取名为 Server 的应用程序运行器

await applicationRunner.StartAsync();   //运行应用程序
```

-------

### 3.4 客户端
```C#
services.AddHoarwell("Client")  //添加名为 Client 的应用程序
        .UseDefaultApplication()    //使用默认应用程序
        .UseDefaultSocketTransportClient(options => options.EndPoints.Add(endPoint))    //使用默认的基于Socket的客户端传输，并配置远程地址为 endPoint（注意此处与服务端不同）
        .UseDefaultSerializer(serializerBuilder =>  //使用默认的对象序列化器
        {
            //添加支持的序列化的消息类型
            //TMessage 需要实现 IBinaryParseable<TMessage> 和 IBinarizable 接口
            //serializerBuilder.AddMessage<TMessage>();
        })
        .UseDefaultTypeIdentifierAnalyzer(typeIdentifierAnalyzerBuilder =>  //使用默认的类型标识符分析器
        {
            //添加支持的消息类型
            //TMessage 需要实现 ITypeIdentifierProvider 接口
            //typeIdentifierAnalyzerBuilder.AddMessage<TMessage>();
        })
        .ConfigureInboundPipeline(pipelineBuilder =>    //配置入站管道
        {
            //配置入站处理管道
            pipelineBuilder.UseUInt32LengthFieldBasedFrameDecoder() //使用基于 uint 的长度帧头解码器中间件
                           .UseDefaultMessageDeserializer() //使用默认的消息序列化中间件
                           .RunEndpoint(endpointBuilder =>  //配置入站终结点
                           {
                                //添加消息的处理器
                                //TMessage 需要已在 Serializer 配置支持
                                //TMessageHandler 需要实现 IEndpointMessageHandler<TMessage> 接口
                                //endpointBuilder.Handle<TMessage, TMessageHandler>();
                           });
        })
        .ConfigureOutboundPipeline(pipelineBuilder =>   //配置出站管道
        {
            //配置出站处理管道
            pipelineBuilder.UseUInt32LengthFieldBasedFrameEncoder() //使用基于 uint 的长度帧头解码器中间件
                           .RunDefaultMessageSerializer();  //运行默认的消息序列化出站终结点
        });

await using var serviceProvider = services.BuildServiceProvider();
await using var applicationRunner = serviceProvider.GetRequiredKeyedService<IHoarwellApplicationRunner>("Client");  //从 DI 容器中获取名为 Client 的应用程序运行器

using var contextWaiter = applicationRunner.GetContextWaiter(); //获取客户端上下文等待器

await applicationRunner.StartAsync();   //启动应用程序

var context = await contextWaiter.Task; //获取客户端上下文

// 使用获取到的 context 进行交互
```

-------

#### 更多信息参见示例代码

-------

## 4. Samples

[ChatRoom](./samples/ChatRoom): 简单的聊天室实现

## 5. Benchmark

[Benchmark](./benchmark)
