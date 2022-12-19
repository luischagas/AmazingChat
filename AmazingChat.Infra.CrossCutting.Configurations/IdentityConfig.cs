using AmazingChat.Infra.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AmazingChat.Infra.CrossCutting.Identity;

public static class IdentityConfig
{
    #region Methods

    public static void AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddDbContext<AmazingChatIdentityContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));

        services.AddDefaultIdentity<IdentityUser>(options => { options.SignIn.RequireConfirmedAccount = false; })
            .AddEntityFrameworkStores<AmazingChatIdentityContext>();
    }

    #endregion Methods
}