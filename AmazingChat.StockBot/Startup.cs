using AmazingChat.Application.Interfaces;
using AmazingChat.Application.Services;
using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Domain.Notification;
using AmazingChat.Domain.Shared.Models;
using AmazingChat.Domain.Shared.Notifications;
using AmazingChat.Domain.Shared.Services;
using AmazingChat.Domain.Shared.UnitOfWork;
using AmazingChat.Infra.CrossCutting.Identity;
using AmazingChat.Infra.CrossCutting.IoC;
using AmazingChat.Infra.CrossCutting.Services.Communication;
using AmazingChat.Infra.CrossCutting.Services.RabbitMQ.Extensions;
using AmazingChat.Infra.Data.Context;
using AmazingChat.Infra.Data.Repositories;
using AmazingChat.Infra.Data.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AmazingChat.StockBot;

public class Startup
{
    #region Constructors

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    #endregion Constructors

    #region Properties

    public IConfiguration Configuration { get; }

    #endregion Properties

    #region Methods

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<AmazingChatContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        services.AddControllers(options =>
            {
                options.Filters.Add(new ServiceFilterAttribute(typeof(GlobalExceptionHandlerFilter)));
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };

                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
        
        services.AddScoped<AmazingChatContext>();
        services.AddScoped<ICommunicationRestService, CommunicationRestService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IRoomMessageRepository, RoomMessageRepository>();
        services.AddScoped<GlobalExceptionHandlerFilter>();
        services.AddScoped<INotifier, Notifier>();
        services.AddScoped<IStockBotService, StockBotService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        
        services.Configure<SignalRConfigurations>(Configuration.GetSection("SignalR"));
        services.Configure<RabbitMqSettings>(Configuration.GetSection("RabbitMQSettings"));
        
        services.AddRabbitMqPublisherService(Configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }


    #endregion Methods
}