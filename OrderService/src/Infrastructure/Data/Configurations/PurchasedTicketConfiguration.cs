using BuildingBlocks.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Data.Configurations;

public class PurchasedTicketConfiguration : IEntityTypeConfiguration<PurchasedTicket>
{
    public void Configure(EntityTypeBuilder<PurchasedTicket> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.TicketId).IsRequired();

        builder.Property(p => p.CustomerId).IsRequired();

        builder.Property(p => p.ExpiredAt).IsRequired();

        builder.Property(p => p.Status).IsRequired();

        builder.ConfigureAuditableProperties();
    }
}
