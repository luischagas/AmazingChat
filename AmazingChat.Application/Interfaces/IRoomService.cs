using AmazingChat.Application.Models;

namespace AmazingChat.Application.Interfaces;

public interface IRoomService
{
    Task<IAppServiceResponse> Create(RoomViewModel request);
    Task<IAppServiceResponse> GetAll();
    Task<IAppServiceResponse> Get(Guid id);
    Task<IAppServiceResponse> Remove(Guid id);

}