using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace Hoarwell;

/// <summary>
/// 对象二进制数据解析帮助类
/// </summary>
public static class ObjectBinaryParseHelper
{
    #region Public 方法

#if NET7_0_OR_GREATER

    /// <summary>
    /// 包装特定类型 <typeparamref name="TMessage"/> 解析委托到解析为 <see cref="object"/>
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="input"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool TryParseAsObject<TMessage>(in ReadOnlySequence<byte> input, [MaybeNullWhen(false)] out object? result)
        where TMessage : IBinaryParseable<TMessage>
    {
        //HACK 无数据时返回default是否符合逻辑
        if (input.IsEmpty)
        {
            result = default(TMessage);
            return true;
        }
        var parseResult = TMessage.TryParse(input, out var typedResult);
        result = typedResult;
        return parseResult;
    }

#endif

    /// <summary>
    /// 包装特定类型 <typeparamref name="TMessage"/> 解析委托到解析为 <see cref="object"/>
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="tryBinaryParseDelegate"></param>
    /// <returns></returns>
    public static TryBinaryParseDelegate<object?> WrapToParseAsObject<TMessage>(this TryBinaryParseDelegate<TMessage> tryBinaryParseDelegate)
    {
        return new ParseAsObjectWrappers<TMessage>(tryBinaryParseDelegate).TryParseAsObject;
    }

    #endregion Public 方法

    #region Private 类

    private sealed class ParseAsObjectWrappers<TMessage>
    {
        #region Private 字段

        private readonly TryBinaryParseDelegate<TMessage> _tryBinaryParseDelegate;

        #endregion Private 字段

        #region Public 构造函数

        public ParseAsObjectWrappers(TryBinaryParseDelegate<TMessage> tryBinaryParseDelegate)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(tryBinaryParseDelegate, nameof(tryBinaryParseDelegate));
            _tryBinaryParseDelegate = tryBinaryParseDelegate;
        }

        #endregion Public 构造函数

        #region Public 方法

        public bool TryParseAsObject(in ReadOnlySequence<byte> input, [MaybeNullWhen(false)] out object? result)
        {
            //HACK 无数据时返回default是否符合逻辑
            if (input.IsEmpty)
            {
                result = default(TMessage);
                return true;
            }
            var parseResult = _tryBinaryParseDelegate(input, out var typedResult);
            result = typedResult;
            return parseResult;
        }

        #endregion Public 方法
    }

    #endregion Private 类
}
