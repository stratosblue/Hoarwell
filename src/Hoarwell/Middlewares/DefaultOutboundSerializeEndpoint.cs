using Microsoft.Extensions.DependencyInjection;

namespace Hoarwell.Middlewares;

internal sealed class DefaultOutboundSerializeEndpoint<TContext>
    : PipelineEndpoint<TContext, OutboundMetadata>
    where TContext : IHoarwellContext
{
    #region Private 字段

    private readonly int _identifierLength;

    private readonly IObjectSerializer _serializer;

    private readonly ITypeIdentifierAnalyzer _typeIdentifierAnalyzer;

    #endregion Private 字段

    #region Public 构造函数

    public DefaultOutboundSerializeEndpoint([ServiceKey] string applicationName,
                                            IServiceProvider serviceProvider)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(applicationName);
        ArgumentNullExceptionHelper.ThrowIfNull(serviceProvider);

        _typeIdentifierAnalyzer = serviceProvider.GetRequiredKeyedService<ITypeIdentifierAnalyzer>(applicationName);
        _serializer = serviceProvider.GetRequiredKeyedService<IObjectSerializer>(applicationName);

        _identifierLength = Convert.ToInt32(_typeIdentifierAnalyzer.TypeIdentifierSize);
    }

    #endregion Public 构造函数

    #region Public 方法

    public override Task InvokeAsync(TContext context, OutboundMetadata input)
    {
        var bufferWriter = input.BufferWriter;

        if (!_typeIdentifierAnalyzer.TryGetIdentifier(input.ValueType, bufferWriter.GetSpan(_identifierLength)))
        {
            throw new MetadataNotFoundException($"Can not found the identifier for type \"{input.ValueType}\"");
        }

        bufferWriter.Advance(_identifierLength);

        _serializer.Serialize(input.ValueType, input.Value, bufferWriter);

        return Task.CompletedTask;
    }

    #endregion Public 方法
}
