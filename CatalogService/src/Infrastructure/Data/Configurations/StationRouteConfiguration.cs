using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

public class StationRouteConfiguration : IEntityTypeConfiguration<StationRoute>
{
    public void Configure(EntityTypeBuilder<StationRoute> builder)
    {
        builder.ToTable("StationRoute");
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
        
        builder.HasOne(sr => sr.EntryStation)
               .WithMany()
               .HasForeignKey(sr => sr.EntryStationId)
               .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(sr => sr.DestinationStation)
               .WithMany()
               .HasForeignKey(sr => sr.DestinationStationId)
               .OnDelete(DeleteBehavior.Restrict);
        
        
        
    }
}
