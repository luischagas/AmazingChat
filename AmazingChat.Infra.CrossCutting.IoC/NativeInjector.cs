using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Infra.Data.Context;
using AmazingChat.Infra.Data.Repositories;
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
        
        return services;
    }

    #endregion Methods
}