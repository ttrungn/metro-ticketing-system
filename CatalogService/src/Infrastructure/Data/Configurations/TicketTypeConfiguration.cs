using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

public class TicketTypeConfiguration : IEntityTypeConfiguration<TicketType>
{
    public void Configure(EntityTypeBuilder<TicketType> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.ExpirationInDays)
            .IsRequired();

        builder.Property(t => t.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        
        builder.HasMany(tt => tt.Tickets)
            .WithOne(t => t.TicketType)
            .HasForeignKey(t => t.TicketTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
