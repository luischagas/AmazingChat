using AmazingChat.Application.Interfaces;
using AmazingChat.Application.Services;
using AmazingChat.Domain.Shared.Models;
using AmazingChat.Infra.CrossCutting.Identity;
using AmazingChat.Infra.CrossCutting.IoC;
using AmazingChat.Infra.CrossCutting.Services.RabbitMQ.Extensions;
using AmazingChat.Infra.CrossCutting.Services.SignalR;
using AmazingChat.Infra.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AmazingChat.StockBot;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<AmazingChatContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        services.AddControllers(options => { options.Filters.Add(new ServiceFilterAttribute(typeof(GlobalExceptionHandlerFilter))); })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };

                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

        services.AddSignalR();

        services.ResolveDependencies();

        services.AddScoped<IStockBotService, StockBotService>();

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
            endpoints.MapHub<ChatHub>("/chatHub");
        });
    }
}