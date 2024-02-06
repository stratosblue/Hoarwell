using Chat.Shared;
using Hoarwell;

namespace ChatRoom.Server;

internal class ConnectPacketMessageHandler : IEndpointMessageHandler<ConnectPacket>
{
    #region Private 字段

    private readonly ChatRoomImpl _chatRoom;

    #endregion Private 字段

    #region Public 构造函数

    public ConnectPacketMessageHandler(ChatRoomImpl chatRoom)
    {
        _chatRoom = chatRoom ?? throw new ArgumentNullException(nameof(chatRoom));
    }

    #endregion Public 构造函数

    #region Public 方法

    public Task HandleAsync(IHoarwellContext context, ConnectPacket? input)
    {
        _chatRoom.Join(context, input!.Name);
        return Task.CompletedTask;
    }

    #endregion Public 方法
}
