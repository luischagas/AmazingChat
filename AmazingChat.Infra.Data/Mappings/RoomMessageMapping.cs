using AmazingChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AmazingChat.Infra.Data.Mappings;

public class RoomMessageMapping : IEntityTypeConfiguration<RoomMessage>
{
    #region Methods

    public void Configure(EntityTypeBuilder<RoomMessage> builder)
    {
        builder
            .ToTable("RoomMessages");

        builder
            .HasKey(i => i.Id).IsClustered(false);

        builder
            .Property(hi => hi.Message)
            .IsRequired();
        
        builder
            .Property(hi => hi.Timestamp)
            .IsRequired();

        builder
            .HasQueryFilter(u => u.IsDeleted == false);

        builder
            .HasOne(s => s.Room)
            .WithMany(d => d.Messages)
            .HasForeignKey(s => s.RoomId)
            .IsRequired();
        
        builder
            .HasOne(s => s.User)
            .WithMany(d => d.Messages)
            .HasForeignKey(s => s.UserId)
            .IsRequired();
        
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