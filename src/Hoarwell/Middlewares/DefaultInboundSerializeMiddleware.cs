using System.Buffers;
using Microsoft.Extensions.DependencyInjection;

namespace Hoarwell.Middlewares;

internal sealed class DefaultInboundSerializeMiddleware<TContext>
    : TransformMiddleware<TContext, ReadOnlySequence<byte>, InboundMetadata>
    where TContext : IHoarwellContext
{
    #region Private 字段

    private readonly int _identifierLength;

    private readonly IObjectSerializer _serializer;

    private readonly ITypeIdentifierAnalyzer _typeIdentifierAnalyzer;

    #endregion Private 字段

    #region Public 构造函数

    public DefaultInboundSerializeMiddleware([ServiceKey] string applicationName,
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

    public override Task<InboundMetadata> InvokeAsync(TContext context, ReadOnlySequence<byte> input)
    {
        var identifierSequence = input.Slice(0, _identifierLength);

        var type = identifierSequence.IsSingleSegment
                   ? GetType(identifierSequence.FirstSpan)
                   : GetType(identifierSequence);

        var value = _serializer.Deserialize(type, input.Slice(_identifierLength));

        return Task.FromResult(new InboundMetadata(value, type));
    }

    #endregion Public 方法

    #region Private 方法

    private Type GetType(ReadOnlySpan<byte> identifier)
    {
        if (!_typeIdentifierAnalyzer.TryGetType(identifier, out var type))
        {
            throw new MetadataNotFoundException($"Can not found the type for identifier \"{identifier.ToDisplay()}\"");
        }
        return type;
    }

    private Type GetType(ReadOnlySequence<byte> identifier)
    {
        Span<byte> identifierSpan = stackalloc byte[_identifierLength];
        identifier.CopyTo(identifierSpan);
        return GetType(identifierSpan);
    }

    #endregion Private 方法
}
