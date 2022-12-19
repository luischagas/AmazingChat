using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AmazingChat.Infra.Data.Context;

public class AmazingChatIdentityContext : IdentityDbContext
{
    #region Constructors

    public AmazingChatIdentityContext(DbContextOptions<AmazingChatIdentityContext> options)
        : base(options)
    {
        Database.Migrate();
    }

    #endregion Constructors
}