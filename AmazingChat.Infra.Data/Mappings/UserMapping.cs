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
            .Property(hi => hi.Email)
            .IsRequired();

        builder
            .Property(hi => hi.ConnectionId);

        builder
            .HasQueryFilter(u => u.IsDeleted == false);

        builder
            .Ignore(d => d.RuleLevelCascadeMode);
        
        builder
            .Ignore(d => d.ClassLevelCascadeMode);
        
        builder
            .Ignore(d => d.CascadeMode);

        builder
            .Ignore(d => d.ValidationResult);

    }

    #endregion Methods
}