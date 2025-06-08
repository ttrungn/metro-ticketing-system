using BuildingBlocks.Infrastructure.Data.Extensions;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;
public class StationRouteConfiguration : IEntityTypeConfiguration<StationRoute>
{
    public void Configure(EntityTypeBuilder<StationRoute> builder)
    {
        builder.Ignore(sr => sr.Id);
        builder.ToTable("StationRoute");
        // 1. Composite PK on (StationId, RouteId)
        builder
            .HasKey(sr => new { sr.StationId, sr.RouteId });

        // 2. Relationships
        builder
            .HasOne(sr => sr.Station)
            .WithMany(s => s.StationRoutes)      // you'll need ICollection<StationRoute> on Station
            .HasForeignKey(sr => sr.StationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(sr => sr.Route)
            .WithMany(r => r.StationRoutes)      // and ICollection<StationRoute> on Route
            .HasForeignKey(sr => sr.RouteId)
            .OnDelete(DeleteBehavior.Cascade);

        // 3. Additional station links
        builder
            .HasOne(sr => sr.EntryStation)
            .WithMany()                          // no back-ref
            .HasForeignKey(sr => sr.EntryStationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(sr => sr.DestinationStation)
            .WithMany()                          // no back-ref
            .HasForeignKey(sr => sr.DestinationStationId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ConfigureAuditableProperties();
    }
}
