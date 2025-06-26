using BuildingBlocks.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Data.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Cart");

        builder.HasKey(c => c.Id);

        // builder.HasKey(c => new { c.CustomerId, c.TicketId });

        builder.Property(c => c.CustomerId)
            .IsRequired();

        builder.Property(c => c.TicketId)
            .IsRequired();

        builder.Property(c => c.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(c => c.EntryStationId)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(c => c.DestinationStationId)
            .IsRequired(false)
            .HasMaxLength(50);
        
        builder.Property(c => c.RouteId)
            .IsRequired(false)
            .HasMaxLength(50);
        
        builder.ConfigureAuditableProperties();
    }
}
