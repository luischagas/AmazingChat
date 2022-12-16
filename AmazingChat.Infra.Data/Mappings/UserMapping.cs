using AmazingChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AmazingChat.Infra.Data.Mappings;

public class UserMapping : IEntityTypeConfiguration<User>
{
    #region Methods

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .ToTable("Users");

        builder
            .HasKey(i => i.Id).IsClustered(false);

        builder
            .Property(hi => hi.Name)
            .IsRequired();

        builder
            .Property(hi => hi.ConnectionId)
            .IsRequired();

        builder
            .HasQueryFilter(u => u.IsDeleted == false);
        
        builder
            .HasMany(i => i.Rooms)
            .WithOne(hi => hi.User)
            .HasForeignKey(b => b.UserId);

        builder
            .Ignore(s => s.ValidationResult);

    }

    #endregion Methods
}