using AmazingChat.Application.Interfaces;
using AmazingChat.Application.Services;
using AmazingChat.Domain.Shared.Models;
using AmazingChat.Infra.CrossCutting.Identity;
using AmazingChat.Infra.CrossCutting.IoC;
using AmazingChat.Infra.CrossCutting.Services.RabbitMQ.Extensions;
using AmazingChat.Infra.CrossCutting.Services.SignalR;
using AmazingChat.Infra.Data.Context;
using AmazingChat.UI.HostedService;
using Microsoft.EntityFrameworkCore;

namespace AmazingChat.UI;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }


    public void ConfigureServices(IServiceCollection services)
    {
        services.AddIdentityConfiguration(Configuration);

        services.AddDbContext<AmazingChatContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        services.AddControllersWithViews()
            .AddRazorPagesOptions(options => { options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout"); });

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
            options.LogoutPath = "/Identity/Account/Logout";
        });

        services.AddRazorPages();

        services.AddSignalR();

        services.AddScoped<IConsumerService, ConsumerService>();

        services.AddRabbitMqConsumerService(Configuration);

        services.AddHostedService<Worker>();

        services.ResolveDependencies();

        services.Configure<SignalRConfigurations>(Configuration.GetSection("SignalR"));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/erro/500");
            app.UseStatusCodePagesWithRedirects("/erro/{0}");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<AuthorizationMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                "default",
                "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapRazorPages();
            endpoints.MapHub<ChatHub>("/chatHub");
        });
    }
}