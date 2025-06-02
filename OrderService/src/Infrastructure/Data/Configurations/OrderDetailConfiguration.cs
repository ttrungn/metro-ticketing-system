using BuildingBlocks.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Data.Configurations;

public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.HasKey(od => new { od.OrderId, od.PurchaseTicketId });

        builder.Property(o => o.OrderId).IsRequired();

        builder.Property(o => o.BoughtPrice).IsRequired();

        builder.HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId);

        builder.HasOne(od => od.PurchasedTicket)
            .WithMany(pt => pt.OrderDetails)
            .HasForeignKey(od => od.PurchaseTicketId);
    }
}
