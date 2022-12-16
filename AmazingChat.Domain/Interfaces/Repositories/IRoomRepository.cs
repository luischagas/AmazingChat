using AmazingChat.Domain.Entities;

namespace AmazingChat.Domain.Interfaces.Repositories;

public interface IRoomRepository : IBaseRepository<Room>
{
    Task<Room> GetByName(string name);
}