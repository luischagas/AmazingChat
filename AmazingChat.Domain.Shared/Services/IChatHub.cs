using AmazingChat.Domain.Shared.Models.SignalR;

namespace AmazingChat.Domain.Shared.Services;

public interface IChatHub
{
    Task SendMessage(MessageModel messageModel);

    Task SendInfoMessage(MessageModel messageModel);

    Task CreateRoom(Guid roomId, string roomName);

    Task RemoveRoom(Guid roomId, string roomName);
}