using System.Buffers;
using System.Collections.Frozen;
using Hoarwell.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hoarwell;

internal sealed class DefaultObjectSerializer
    : IObjectSerializer
{
    #region Private 字段

    private readonly FrozenDictionary<Type, TryBinaryParseDelegate<object?>> _tryBinaryParseAsObjectDelegateMap;

    #endregion Private 字段

    #region Public 构造函数

    public DefaultObjectSerializer([ServiceKey] string applicationName,
                                   IOptionsMonitor<DefaultHoarwellSerializerOptions> optionsMonitor)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(applicationName);

        _tryBinaryParseAsObjectDelegateMap = optionsMonitor.GetRequiredApplicationOptions(applicationName, options => options.TryBinaryParseAsObjectDelegateMap)
                                                           .ToFrozenDictionary();
    }

    #endregion Public 构造函数

    #region Public 方法

    public object? Deserialize(Type type, ReadOnlySequence<byte> data)
    {
        if (!_tryBinaryParseAsObjectDelegateMap[type](data, out var result))
        {
            throw new InvalidOperationException($"Can not parse the data for type \"{type}\"");
        }
        return result;
    }

    public void Serialize(Type type, object? value, IBufferWriter<byte> bufferWriter)
    {
        //HACK null时直接不写入是否符合逻辑
        if (value is null)
        {
            return;
        }
        if (value is not IBinarizable binarizable)
        {
            throw new InvalidOperationException($"The serializer \"{GetType()}\" only support serialize the object derive from \"{nameof(IBinarizable)}\"");
        }
        binarizable.Serialize(bufferWriter);
    }

    #endregion Public 方法
}
