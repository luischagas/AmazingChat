using AmazingChat.Domain.Entities;
using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AmazingChat.Infra.Data.Repositories;

public class RoomMessageRepository : BaseRepository<RoomMessage>, IRoomMessageRepository
{
    private readonly DbSet<RoomMessage> _roomMessages;

    public RoomMessageRepository(AmazingChatContext db) : base(db)
    {
        _roomMessages = db.Set<RoomMessage>();
    }

    public async Task<IEnumerable<RoomMessage>> GetAllByRoomAsync(Guid roomId)
    {
        return await _roomMessages
            .Include(r => r.Room)
            .Include(r => r.User)
            .Where(a => a.RoomId == roomId)
            .OrderByDescending(m => m.Timestamp)
            .Take(50)
            .Reverse()
            .ToListAsync();
    }

    public async Task<IEnumerable<RoomMessage>> GetAllDetailedAsync()
    {
        return await _roomMessages
            .Include(r => r.Room)
            .Include(r => r.User)
            .OrderByDescending(m => m.Timestamp)
            .Take(20)
            .Reverse()
            .ToListAsync();
    }
}