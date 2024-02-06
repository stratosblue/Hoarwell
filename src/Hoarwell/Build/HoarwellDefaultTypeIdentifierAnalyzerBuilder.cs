using Hoarwell.Enhancement;
using Hoarwell.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hoarwell.Build;

/// <summary>
/// Hoarwell 默认类型标识符构建器
/// </summary>
public sealed class HoarwellDefaultTypeIdentifierAnalyzerBuilder
{
    #region Private 字段

    private int? _typeIdentifierSizeRecord;

    #endregion Private 字段

    #region Public 属性

    /// <summary>
    /// Hoarwell 构建器
    /// </summary>
    public HoarwellBuilder HoarwellBuilder { get; }

    #endregion Public 属性

    #region Internal 属性

    internal Dictionary<ReadOnlyMemory<byte>, Type> IdentifierToTypeMap { get; } = new(ReadOnlyMemoryByteEqualityComparer.Shared);

    internal Dictionary<Type, ReadOnlyMemory<byte>> TypeToIdentifierMap { get; } = [];

    #endregion Internal 属性

    #region Public 构造函数

    /// <inheritdoc cref="HoarwellDefaultTypeIdentifierAnalyzerBuilder"/>
    public HoarwellDefaultTypeIdentifierAnalyzerBuilder(HoarwellBuilder hoarwellBuilder)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(hoarwellBuilder);

        HoarwellBuilder = hoarwellBuilder;
    }

    #endregion Public 构造函数

    #region Public 方法

#if NET7_0_OR_GREATER

    /// <summary>
    /// 添加可分析的消息
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <returns></returns>
    public HoarwellDefaultTypeIdentifierAnalyzerBuilder AddMessage<TMessage>()
        where TMessage : ITypeIdentifierProvider
    {
        var identifier = TMessage.TypeIdentifier.ToArray();

        return AddMessage(typeof(TMessage), identifier);
    }

#endif

    /// <summary>
    /// 添加可分析的消息
    /// </summary>
    /// <param name="type"></param>
    /// <param name="identifier"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public HoarwellDefaultTypeIdentifierAnalyzerBuilder AddMessage(Type type, ReadOnlyMemory<byte> identifier)
    {
        _typeIdentifierSizeRecord ??= identifier.Length;

        if (_typeIdentifierSizeRecord.Value != identifier.Length)
        {
            throw new InvalidOperationException($"The identifier length for type \"{type}\" is \"{identifier.Length}\" not match the existed length \"{_typeIdentifierSizeRecord.Value}\"");
        }

        if (IdentifierToTypeMap.TryGetValue(identifier, out var existedType)
            && existedType != type)
        {
            throw new InvalidOperationException($"The identifier for type \"{type}\" has added for type \"{existedType}\"");
        }

        if (TypeToIdentifierMap.TryGetValue(type, out var existedIdentifier)
            && !existedIdentifier.Span.SequenceEqual(identifier.Span))
        {
            throw new InvalidOperationException($"The type \"{type}\" has added as another identifier \"{existedIdentifier.Span.ToDisplay()}\"");
        }

        IdentifierToTypeMap.TryAdd(identifier, type);
        TypeToIdentifierMap.TryAdd(type, identifier);

        return this;
    }

    /// <summary>
    /// 构建到 <paramref name="services"/> 中
    /// </summary>
    /// <param name="services"></param>
    public void Build(IServiceCollection services)
    {
        services.TryAddKeyedSingleton<ITypeIdentifierAnalyzer, DefaultTypeIdentifierAnalyzer>(HoarwellBuilder.ApplicationName);

        var typeToIdentifierMap = TypeToIdentifierMap;
        var identifierToTypeMap = IdentifierToTypeMap;

        services.AddOptions<DefaultTypeIdentifierAnalyzerOptions>(HoarwellBuilder.ApplicationName)
                .Configure(options =>
                {
                    options.TypeToIdentifierMap = typeToIdentifierMap;
                    options.IdentifierToTypeMap = identifierToTypeMap;
                });
    }

    #endregion Public 方法
}
