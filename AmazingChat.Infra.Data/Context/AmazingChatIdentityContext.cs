using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace AmazingChat.Infra.Data.Context;

public class AmazingChatIdentityContext : IdentityDbContext
{
    #region Constructors
    
    public AmazingChatIdentityContext(DbContextOptions<AmazingChatIdentityContext> options)
        : base(options)
    {
    }

    #endregion Constructors
}