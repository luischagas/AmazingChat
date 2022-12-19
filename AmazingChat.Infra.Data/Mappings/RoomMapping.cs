using AmazingChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AmazingChat.Infra.Data.Mappings;

public class RoomMapping : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder
            .ToTable("Rooms");

        builder
            .HasKey(i => i.Id).IsClustered(false);

        builder
            .Property(hi => hi.Name)
            .IsRequired();

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
}