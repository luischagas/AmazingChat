using AmazingChat.Application.Common;
using AmazingChat.Application.Interfaces;
using AmazingChat.Application.Models;
using AmazingChat.Domain.Entities;
using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Domain.Shared.Notifications;
using AmazingChat.Domain.Shared.UnitOfWork;

namespace AmazingChat.Application.Services;

public class UserService : AppService, IUserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUnitOfWork unitOfWork,
        INotifier notifier,
        IUserRepository userRepository)
        : base(unitOfWork, notifier)
    {
        _userRepository = userRepository;
    }

    public async Task<IAppServiceResponse> Create(UserViewModel request)
    {
        var existentUser = await _userRepository.GetByEmail(request.Email);

        if (existentUser is not null)
        {
            Notify("Users", "User existent");

            return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error to Create User", false));
        }

        var user = new User(request.Id, request.Email);

        if (user.IsValid())
        {
            await _userRepository.AddAsync(user);
        }
        else
        {
            Notify(user.ValidationResult);

            return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error to Create User", false));
        }

        if (await CommitAsync())
            return await Task.FromResult(new AppServiceResponse<UserViewModel>(new UserViewModel { Id = user.Id, Email = user.Email }, "User Created Successfully", true));

        return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error Creating User", false));
    }
}