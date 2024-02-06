namespace Hoarwell.Options;

internal sealed class DefaultTypeIdentifierAnalyzerOptions
{
    #region Public 属性

    public Dictionary<ReadOnlyMemory<byte>, Type>? IdentifierToTypeMap { get; set; }

    public Dictionary<Type, ReadOnlyMemory<byte>>? TypeToIdentifierMap { get; set; }

    #endregion Public 属性
}
