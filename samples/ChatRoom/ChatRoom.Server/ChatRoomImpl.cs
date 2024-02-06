using System.Collections.Concurrent;
using Chat.Shared;
using Hoarwell;
using Microsoft.Extensions.DependencyInjection;

namespace ChatRoom.Server;

internal class ChatRoomImpl
{
    #region Public 属性

    public ConcurrentDictionary<IHoarwellContext, string> Users { get; set; } = new();

    #endregion Public 属性

    #region Public 方法

    public Task BroadcastAsync(ChatPacket chatPacket)
    {
        foreach (var item in Users)
        {
            item.Key.WriteAndFlushAsync(chatPacket);
        }
        return Task.CompletedTask;
    }

    public void Exist(IHoarwellContext context)
    {
        if (Users.TryGetValue(context, out var value))
        {
            Console.WriteLine($"User {value} disconnected");
            BroadcastAsync(new ChatPacket() { Name = "System", Message = $"User {value} existed" });
        }
        context.Dispose();
    }

    public void Join(IHoarwellContext context, string name)
    {
        if (!Users.TryAdd(context, name))
        {
            context.WriteAndFlushAsync(new ChatPacket() { Name = "System", Message = $"There name \"{name}\" was existed" }).ContinueWith(_ => context.Abort());
            return;
        }

        context.ExecutionAborted.Register(() => Exist(context));

        Console.WriteLine($"User {name} connected");
        BroadcastAsync(new ChatPacket() { Name = "System", Message = $"User {name} joined" });
    }

    #endregion Public 方法
}
