using AmazingChat.Domain.Entities;

namespace AmazingChat.Domain.Interfaces.Repositories;

public interface IRoomMessageRepository : IBaseRepository<RoomMessage>
{
    Task<IEnumerable<RoomMessage>> GetAllByRoomAsync(Guid roomId);

    Task<IEnumerable<RoomMessage>> GetAllDetailedAsync();
}