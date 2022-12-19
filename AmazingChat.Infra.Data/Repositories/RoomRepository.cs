using AmazingChat.Domain.Entities;
using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AmazingChat.Infra.Data.Repositories;

public class RoomRepository : BaseRepository<Room>, IRoomRepository
{
    private readonly DbSet<Room> _rooms;

    public RoomRepository(AmazingChatContext db) : base(db)
    {
        _rooms = db.Set<Room>();
    }


    public async Task<Room> GetByName(string name)
    {
        return await _rooms
            .FirstOrDefaultAsync(a => a.Name == name);
    }
}