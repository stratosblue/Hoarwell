using Chat.Shared;
using Hoarwell;

namespace ChatRoom.Client;

internal class ChatPacketMessageHandler : IEndpointMessageHandler<ChatPacket>
{
    #region Public 方法

    public Task HandleAsync(IHoarwellContext context, ChatPacket? input)
    {
        Console.WriteLine($"{input.Name}: {input.Message}");
        return Task.CompletedTask;
    }

    #endregion Public 方法
}
