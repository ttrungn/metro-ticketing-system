using BuildingBlocks.Infrastructure.Data.Extensions;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

public class LineConfiguration : IEntityTypeConfiguration<Line>
{
    public void Configure(EntityTypeBuilder<Line> builder)
    {
        builder.ToTable("Line");
        builder.ConfigureAuditableProperties();
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(l => l.TotalLengthInKm)
            .IsRequired();

        builder.Property(l => l.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        // Relationship Route
        builder.HasOne(l => l.Route)
            .WithOne(r => r.Line)
            .HasForeignKey<Line>(l => l.RouteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship EntryStation
        builder.HasOne(l => l.EntryStation)
            .WithMany(l => l.EntryLines)
            .HasForeignKey(l => l.EntryStationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship ExitStation
        builder.HasOne(l => l.ExitStation)
            .WithMany(s => s.ExitLines)
            .HasForeignKey(l => l.ExitStationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
