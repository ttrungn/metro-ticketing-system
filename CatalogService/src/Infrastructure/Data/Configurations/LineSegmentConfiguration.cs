using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

public class LineSegmentConfiguration : IEntityTypeConfiguration<LineSegment>
{
    public void Configure(EntityTypeBuilder<LineSegment> builder)
    {
        builder.HasKey(ls => ls.Id);

        builder.Property(ls => ls.LengthInKm)
            .IsRequired();

        builder.Property(ls => ls.Order)
            .IsRequired();

        // Relationship FromStation
        builder.HasOne(ls => ls.FromStation)
            .WithMany(s => s.FromSegments)
            .HasForeignKey(ls => ls.FromStationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship ToStation
        builder.HasOne(ls => ls.ToStation)
            .WithMany(s => s.ToSegments)
            .HasForeignKey(ls => ls.ToStationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
