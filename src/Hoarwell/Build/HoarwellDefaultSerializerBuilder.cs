using Hoarwell.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hoarwell.Build;

/// <summary>
/// Hoarwell 默认序列化器构建器
/// </summary>
public sealed class HoarwellDefaultSerializerBuilder
{
    #region Public 属性

    /// <summary>
    /// Hoarwell 构建器
    /// </summary>
    public HoarwellBuilder HoarwellBuilder { get; }

    #endregion Public 属性

    #region Internal 属性

    internal Dictionary<Type, TryBinaryParseDelegate<object?>> TryBinaryParseAsObjectDelegateMap { get; } = [];

    #endregion Internal 属性

    #region Public 构造函数

    /// <inheritdoc cref="HoarwellDefaultSerializerBuilder"/>
    public HoarwellDefaultSerializerBuilder(HoarwellBuilder hoarwellBuilder)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(hoarwellBuilder);

        HoarwellBuilder = hoarwellBuilder;
    }

    #endregion Public 构造函数

    #region Public 方法

#if NET7_0_OR_GREATER

    /// <summary>
    /// 添加可序列化消息
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <returns></returns>
    public HoarwellDefaultSerializerBuilder AddMessage<TMessage>()
        where TMessage : IBinaryParseable<TMessage>, IBinarizable
    {
        return AddMessage(typeof(TMessage), ObjectBinaryParseHelper.TryParseAsObject<TMessage>);
    }

#endif

    /// <summary>
    /// 添加可序列化消息
    /// </summary>
    /// <param name="type"></param>
    /// <param name="tryBinaryParseDelegate"></param>
    /// <returns></returns>
    public HoarwellDefaultSerializerBuilder AddMessage(Type type, TryBinaryParseDelegate<object?> tryBinaryParseDelegate)
    {
        TryBinaryParseAsObjectDelegateMap.Add(type, tryBinaryParseDelegate);
        return this;
    }

    /// <summary>
    /// 构建到 <paramref name="services"/> 中
    /// </summary>
    /// <param name="services"></param>
    public void Build(IServiceCollection services)
    {
        var tryBinaryParseAsObjectDelegateMap = TryBinaryParseAsObjectDelegateMap;

        services.AddOptions<DefaultHoarwellSerializerOptions>(HoarwellBuilder.ApplicationName)
                .Configure(options =>
                {
                    options.TryBinaryParseAsObjectDelegateMap = tryBinaryParseAsObjectDelegateMap;
                });

        services.TryAddKeyedSingleton<IObjectSerializer, DefaultObjectSerializer>(HoarwellBuilder.ApplicationName);
    }

    #endregion Public 方法
}
