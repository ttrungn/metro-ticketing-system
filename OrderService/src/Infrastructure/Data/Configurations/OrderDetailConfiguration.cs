using BuildingBlocks.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Data.Configurations;

public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.HasKey(od => od.Id);

        builder.ToTable("OrderDetail");

        builder.Property(o => o.OrderId).IsRequired();

        builder.Property(o => o.BoughtPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(o => o.ActiveAt).IsRequired();

        builder.Property(o => o.ExpiredAt).IsRequired();

        builder.Property(o => o.Status).IsRequired();

        builder.Property(o => o.EntryStationId)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(o => o.DestinationStationId)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId);

        builder.ConfigureAuditableProperties();
    }
}
