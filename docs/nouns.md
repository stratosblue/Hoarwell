# 名词列表

|类型名称|名称|作用|
|---|---|---|
|ApplicationRunner|应用程序运行程序|连接 DuplexPipeConnectorFactory 与 HoarwellApplication ，启动或停止应用|
|HoarwellApplication|Hoarwell应用程序|数据处理 ExecutionPipeline 的执行者|
|DuplexPipeConnectorFactory|双工管道连接器工厂|创建 DuplexPipeConnector|
|DuplexPipeConnector|双工管道连接器|创建 DuplexPipeContext|
|DuplexPipeContext|双工管道上下文|提供 Inputter 与 Outputter 及其相关上下文信息|
|HoarwellContext|Hoarwell上下文|在单个双工管道处理流程中传递上下文，控制管道状态，传递消息|
|Inputter|输入器|处理数据输入的类型（由`DuplexPipeConnectorFactory`确定其类型）|
|Outputter|输出器|处理数据输出的类型（由`DuplexPipeConnectorFactory`确定其类型）||
|OutputterAdapter|输出器适配器|将输出器`Outputter`适配为标准输出接口`IOutputter`实现|
|IOutputter|标准输出器接|适配后的`Outputter`|
|ExecutionPipeline|执行管道|数据处理管道|
|InboundPipeline|输入执行管道|接收处理 Inputter 的数据，并处理执行直至 Endpoint|
|OutboundPipeline|输出执行管道|接收处理 Endpoint 的数据，并处理传递给 Outputter|
|Endpoint|终结点|接收已解析的消息，并调用对应的 EndpointMessageHandler|
|EndpointMessageHandler|终结点消息处理器|执行最终的用户逻辑|
||||
|ObjectSerializer|对象序列化器|将需要发送的对象序列化为二进制数据或从二进制数据反序列化为对象|
|TypeIdentifierAnalyzer|类型标识符分析器|内置的`ObjectSerializer`附属实现，用于确定类型的唯一标识符或从二进制数据读取唯一标识符，以序列化或确定反序列化的目标类型|
