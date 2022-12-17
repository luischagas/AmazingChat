using AmazingChat.Application.Interfaces;
using AmazingChat.Application.Services;
using AmazingChat.Domain.Interfaces.Notifier;
using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Domain.Notification;
using AmazingChat.Domain.Shared;
using AmazingChat.Infra.CrossCutting.Services.SignalR;
using AmazingChat.Infra.Data.Context;
using AmazingChat.Infra.Data.Repositories;
using AmazingChat.Infra.Data.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace AmazingChat.Infra.CrossCutting.IoC;

public static class NativeInjector
{
    #region Methods

    public static IServiceCollection ResolveDependencies(this IServiceCollection services)
    {
        services.AddScoped<AmazingChatContext>();
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IRoomMessageRepository, RoomMessageRepository>();
        
        
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMessageService, MessageService>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<INotifier, Notifier>();
        
        services.AddScoped<ChatHub>();

        return services;
    }

    #endregion Methods
}