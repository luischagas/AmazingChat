using AmazingChat.Application.Interfaces;
using AmazingChat.Application.Services;
using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Domain.Notification;
using AmazingChat.Domain.Shared.Notifications;
using AmazingChat.Domain.Shared.Services;
using AmazingChat.Domain.Shared.UnitOfWork;
using AmazingChat.Infra.CrossCutting.Identity;
using AmazingChat.Infra.CrossCutting.Services.Communication;
using AmazingChat.Infra.CrossCutting.Services.SignalR;
using AmazingChat.Infra.Data.Context;
using AmazingChat.Infra.Data.Repositories;
using AmazingChat.Infra.Data.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AmazingChat.Infra.CrossCutting.IoC;

public static class NativeInjector
{
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
        services.AddScoped<ICommunicationRestService, CommunicationRestService>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddScoped<IChatHub, ChatHub>();

        services.AddScoped<GlobalExceptionHandlerFilter>();

        return services;
    }
}