using BuildingBlocks.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.CustomerId).IsRequired();

        builder.Property(o => o.ThirdPartyPaymentId).IsRequired();

        builder.Property(o => o.PaymentMethod).IsRequired();

        builder.Property(o => o.Status).IsRequired();

        builder.ConfigureAuditableProperties();
    }
}
