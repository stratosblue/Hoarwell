using Chat.Shared;
using Hoarwell;

namespace ChatRoom.Server;

internal class ChatPacketMessageHandler : IEndpointMessageHandler<ChatPacket>
{
    #region Private 字段

    private readonly ChatRoomImpl _chatRoom;

    #endregion Private 字段

    #region Public 构造函数

    public ChatPacketMessageHandler(ChatRoomImpl chatRoom)
    {
        _chatRoom = chatRoom;
    }

    #endregion Public 构造函数

    #region Public 方法

    public Task HandleAsync(IHoarwellContext context, ChatPacket? input)
    {
        return _chatRoom.BroadcastAsync(input!);
    }

    #endregion Public 方法
}
