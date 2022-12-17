using AmazingChat.Application.Models;

namespace AmazingChat.Application.Interfaces;

public interface IMessageService
{
    Task<IAppServiceResponse> Create(MessageViewModel request);
    Task<IAppServiceResponse> GetAll();
    Task<IAppServiceResponse> GetByRoom(string roomName);

}