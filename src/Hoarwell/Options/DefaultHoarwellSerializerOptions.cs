namespace Hoarwell.Options;

internal sealed class DefaultHoarwellSerializerOptions
{
    #region Public 属性

    internal Dictionary<Type, TryBinaryParseDelegate<object?>>? TryBinaryParseAsObjectDelegateMap { get; set; }

    #endregion Public 属性
}
