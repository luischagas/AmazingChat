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
    
    #endregion Public Methods
}