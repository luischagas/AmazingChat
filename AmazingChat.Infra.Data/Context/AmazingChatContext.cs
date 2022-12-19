using AmazingChat.Infra.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AmazingChat.Infra.Data.Context;

public class AmazingChatContext : DbContext
{
    public AmazingChatContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetProperties()
                         .Where(p => p.ClrType == typeof(string))))
            property.SetColumnType("varchar(100)");

        modelBuilder.ApplyConfiguration(new UserMapping());
        modelBuilder.ApplyConfiguration(new RoomMapping());
        modelBuilder.ApplyConfiguration(new RoomMessageMapping());
    }
}