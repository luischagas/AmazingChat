using AmazingChat.Domain.Entities;

namespace AmazingChat.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User> GetByEmail(string email);
}