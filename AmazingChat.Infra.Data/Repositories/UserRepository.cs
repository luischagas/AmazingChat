using AmazingChat.Domain.Entities;
using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AmazingChat.Infra.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
    {
        #region Private Fields

        private readonly DbSet<User> _users;

        #endregion Private Fields

        #region Public Constructors

        public UserRepository(AmazingChatContext db) : base(db)
        {
            _users = db.Set<User>();
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task<User> GetByConnectionId(string connectionId)
        {
            return await _users
                .FirstOrDefaultAsync(a => a.ConnectionId == connectionId);

        }

        #endregion Public Methods
    }