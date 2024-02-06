using System.Buffers;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using Hoarwell.Enhancement;
using Hoarwell.Enhancement.Buffers;
using Hoarwell.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hoarwell;

internal sealed class DefaultTypeIdentifierAnalyzer : ITypeIdentifierAnalyzer
{
    #region Private 字段

    private readonly ITypeIdentifierAnalyzer _innerTypeIdentifierAnalyzer;

    #endregion Private 字段

    #region Public 属性

    /// <inheritdoc/>
    public uint TypeIdentifierSize { get; }

    #endregion Public 属性

    #region Public 构造函数

    public DefaultTypeIdentifierAnalyzer([ServiceKey] string applicationName,
                                         IOptionsMonitor<DefaultTypeIdentifierAnalyzerOptions> optionsMonitor)
    {
        var typeToIdentifierMap = optionsMonitor.GetRequiredApplicationOptions(applicationName, options => options.TypeToIdentifierMap);

        var identifierToTypeMap = optionsMonitor.GetRequiredApplicationOptions(applicationName, options => options.IdentifierToTypeMap);

        if (identifierToTypeMap.GroupBy(static m => m.Key.Length).Count() != 1)
        {
            throw new InvalidOperationException("The identifier length unaligned");
        }

        TypeIdentifierSize = (uint)identifierToTypeMap.First().Key.Length;

        _innerTypeIdentifierAnalyzer = TypeIdentifierSize <= Int32BaseTypeIdentifierAnalyzerImpl.MaxSupportIdentifierSize
                                       ? new Int32BaseTypeIdentifierAnalyzerImpl(TypeIdentifierSize, identifierToTypeMap, typeToIdentifierMap)
                                       : new ReadOnlyMemoryBaseTypeIdentifierAnalyzerImpl(TypeIdentifierSize, identifierToTypeMap, typeToIdentifierMap);
    }

    #endregion Public 构造函数

    #region Public 方法

    public bool TryGetIdentifier(in Type type, Span<byte> destination) => _innerTypeIdentifierAnalyzer.TryGetIdentifier(type, destination);

    public bool TryGetIdentifier(in ReadOnlySequence<byte> input, Span<byte> destination) => _innerTypeIdentifierAnalyzer.TryGetIdentifier(input, destination);

    public bool TryGetIdentifier(in Type type, out ReadOnlyMemory<byte> identifier) => _innerTypeIdentifierAnalyzer.TryGetIdentifier(type, out identifier);

    public bool TryGetIdentifier(in ReadOnlySequence<byte> input, out ReadOnlyMemory<byte> identifier) => _innerTypeIdentifierAnalyzer.TryGetIdentifier(input, out identifier);

    public bool TryGetType(in ReadOnlySpan<byte> identifier, [NotNullWhen(true)] out Type? type) => _innerTypeIdentifierAnalyzer.TryGetType(identifier, out type);

    #endregion Public 方法

    #region Private 类

    private sealed class Int32BaseTypeIdentifierAnalyzerImpl
        : ITypeIdentifierAnalyzer
    {
        #region Private 字段

        private readonly FrozenDictionary<int, ReadOnlyMemory<byte>> _identifierInt32ToMemoryMap;

        private readonly FrozenDictionary<int, Type> _identifierToTypeMap;

        private readonly FrozenDictionary<Type, ReadOnlyMemory<byte>> _typeToIdentifierMap;

        #endregion Private 字段

        #region Public 字段

        public const int MaxSupportIdentifierSize = sizeof(int);

        #endregion Public 字段

        #region Public 属性

        /// <inheritdoc/>
        public uint TypeIdentifierSize { get; }

        #endregion Public 属性

        #region Public 构造函数

        public Int32BaseTypeIdentifierAnalyzerImpl(uint typeIdentifierSize,
                                                   IDictionary<ReadOnlyMemory<byte>, Type> identifierToTypeMap,
                                                   IDictionary<Type, ReadOnlyMemory<byte>> typeToIdentifierMap)
        {
            TypeIdentifierSize = typeIdentifierSize;

            _typeToIdentifierMap = typeToIdentifierMap.ToFrozenDictionary();

            _identifierToTypeMap = identifierToTypeMap.ToFrozenDictionary(keySelector: static m => SmallDataAlignReadUtil.UnsafeReadAsInt32(m.Key.Span),
                                                                          elementSelector: static m => m.Value);

            _identifierInt32ToMemoryMap = identifierToTypeMap.ToFrozenDictionary(keySelector: static m => SmallDataAlignReadUtil.UnsafeReadAsInt32(m.Key.Span),
                                                                                 elementSelector: static m => m.Key);
        }

        #endregion Public 构造函数

        #region Public 方法

        public bool TryGetIdentifier(in Type type, Span<byte> destination)
        {
            if (_typeToIdentifierMap.TryGetValue(type, out var value))
            {
                value.Span.CopyTo(destination);
                return true;
            }
            return false;
        }

        public bool TryGetIdentifier(in ReadOnlySequence<byte> input, Span<byte> destination)
        {
            if (input.Length >= TypeIdentifierSize)
            {
                input.Slice(0, TypeIdentifierSize).CopyTo(destination);
                return true;
            }
            return false;
        }

        public bool TryGetIdentifier(in Type type, out ReadOnlyMemory<byte> identifier)
        {
            return _typeToIdentifierMap.TryGetValue(type, out identifier);
        }

        public bool TryGetIdentifier(in ReadOnlySequence<byte> input, out ReadOnlyMemory<byte> identifier)
        {
            if (input.Length >= TypeIdentifierSize
                && _identifierInt32ToMemoryMap.TryGetValue(SmallDataAlignReadUtil.UnsafeReadAsInt32(input), out identifier))
            {
                return true;
            }
            identifier = default;
            return false;
        }

        public bool TryGetType(in ReadOnlySpan<byte> identifier, [NotNullWhen(true)] out Type? type)
        {
            return _identifierToTypeMap.TryGetValue(SmallDataAlignReadUtil.UnsafeReadAsInt32(identifier), out type);
        }

        #endregion Public 方法
    }

    private sealed class ReadOnlyMemoryBaseTypeIdentifierAnalyzerImpl(uint typeIdentifierSize,
                                                                      IDictionary<ReadOnlyMemory<byte>, Type> identifierToTypeMap,
                                                                      IDictionary<Type, ReadOnlyMemory<byte>> typeToIdentifierMap)
        : ITypeIdentifierAnalyzer
    {
        #region Private 字段

        private readonly FrozenDictionary<ReadOnlyMemory<byte>, Type> _identifierToTypeMap = identifierToTypeMap.ToFrozenDictionary(ReadOnlyMemoryByteEqualityComparer.Shared);

        private readonly FrozenDictionary<Type, ReadOnlyMemory<byte>> _typeToIdentifierMap = typeToIdentifierMap.ToFrozenDictionary();

        #endregion Private 字段

        #region Public 属性

        /// <inheritdoc/>
        public uint TypeIdentifierSize { get; } = typeIdentifierSize;

        #endregion Public 属性

        #region Public 方法

        public bool TryGetIdentifier(in Type type, Span<byte> destination)
        {
            if (_typeToIdentifierMap.TryGetValue(type, out var value))
            {
                value.Span.CopyTo(destination);
                return true;
            }
            return false;
        }

        public bool TryGetIdentifier(in ReadOnlySequence<byte> input, Span<byte> destination)
        {
            if (input.Length >= TypeIdentifierSize)
            {
                input.Slice(0, TypeIdentifierSize).CopyTo(destination);
                return true;
            }
            return false;
        }

        public bool TryGetIdentifier(in Type type, out ReadOnlyMemory<byte> identifier)
        {
            return _typeToIdentifierMap.TryGetValue(type, out identifier);
        }

        public bool TryGetIdentifier(in ReadOnlySequence<byte> input, out ReadOnlyMemory<byte> identifier)
        {
            if (input.Length >= TypeIdentifierSize)
            {
                // Additional performance overhead about ToArray()
                identifier = input.Slice(0, TypeIdentifierSize).ToArray();
                return true;
            }
            identifier = default;
            return false;
        }

        public bool TryGetType(in ReadOnlySpan<byte> identifier, [NotNullWhen(true)] out Type? type)
        {
            // Additional performance overhead about ToArray()
            return _identifierToTypeMap.TryGetValue(identifier.ToArray(), out type);
        }

        #endregion Public 方法
    }

    #endregion Private 类
}
