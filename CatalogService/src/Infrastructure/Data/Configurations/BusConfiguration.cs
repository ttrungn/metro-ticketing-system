using BuildingBlocks.Infrastructure.Data.Extensions;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

public class BusConfiguration : IEntityTypeConfiguration<Bus>
{
    public void Configure(EntityTypeBuilder<Bus> builder)
    {
        builder.ConfigureAuditableProperties();
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.DestinationName)
            .HasMaxLength(200);

        builder.HasOne(b => b.Station)
            .WithMany(s => s.Buses)
            .HasForeignKey(b => b.StationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
