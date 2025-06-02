using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

public class StationRouteConfiguration : IEntityTypeConfiguration<StationRoute>
{
    public void Configure(EntityTypeBuilder<StationRoute> builder)
    {
        // Composite key
        builder.HasKey(sr => new { sr.StationId, sr.RouteId });

        // Relationship StationRoute -> Station
        builder.HasOne(sr => sr.Station)
            .WithMany(s => s.StationRoutes)
            .HasForeignKey(sr => sr.StationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship StationRoute -> Route
        builder.HasOne(sr => sr.Route)
            .WithMany(r => r.StationRoutes)
            .HasForeignKey(sr => sr.RouteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
