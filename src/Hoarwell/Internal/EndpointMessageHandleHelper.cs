using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Hoarwell.Internal;

internal static class EndpointMessageHandleHelper
{
    #region Public 方法

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task HandleMessageAsync<TMessage, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMessageHandler>(IHoarwellContext context, InboundMetadata input)
         where TMessageHandler : IEndpointMessageHandler<TMessage>
    {
        var messageHandler = context.Services.GetRequiredKeyedService<TMessageHandler>(context.ApplicationName);

        return messageHandler.HandleAsync(context, (TMessage?)input.Value);
    }

    #endregion Public 方法
}
