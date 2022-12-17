using AmazingChat.Domain.Entities;
using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AmazingChat.Infra.Data.Repositories;

public class RoomMessageRepository: BaseRepository<RoomMessage>, IRoomMessageRepository
{
    #region Private Fields

    private readonly DbSet<RoomMessage> _roomMessages;

    #endregion Private Fields

    #region Public Constructors

    public RoomMessageRepository(AmazingChatContext db) : base(db)
    {
        _roomMessages = db.Set<RoomMessage>();
    }

    #endregion Public Constructors

    #region Public Methods
    
    public async Task<IEnumerable<RoomMessage>> GetAllByRoomAsync(Guid roomId)
    {
        return await _roomMessages
            .Include(r => r.Room)
            .Include(r => r.User)
            .Where(a => a.RoomId == roomId)
            .OrderByDescending(m => m.Timestamp)
            .Take(20)
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
    
    #endregion Public Methods
}