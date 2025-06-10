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

        builder.Ignore(c => c.Id);

        builder.HasKey(c => new { c.CustomerId, c.TicketId });

        builder.Property(c => c.CustomerId)
            .IsRequired();

        builder.Property(c => c.TicketId)
            .IsRequired();

        builder.ConfigureAuditableProperties();
    }
}
