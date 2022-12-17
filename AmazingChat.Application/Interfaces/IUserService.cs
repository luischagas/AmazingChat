using AmazingChat.Application.Models;

namespace AmazingChat.Application.Interfaces;

public interface IUserService
{
    Task<IAppServiceResponse> Create(UserViewModel request);

}