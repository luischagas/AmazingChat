using AmazingChat.Domain.Entities;
using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AmazingChat.Infra.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly DbSet<User> _users;
    
    public UserRepository(AmazingChatContext db) : base(db)
    {
        _users = db.Set<User>();
    }
    
    public async Task<User> GetByEmail(string email)
    {
        return await _users
            .FirstOrDefaultAsync(a => a.Email == email);
    }
}