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
        builder
            .HasKey(sr => new { sr.StationId, sr.RouteId });
        
        builder
            .HasOne(sr => sr.Station)
            .WithMany(s => s.StationRoutes)    
            .HasForeignKey(sr => sr.StationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(sr => sr.Route)
            .WithMany(r => r.StationRoutes)     
            .HasForeignKey(sr => sr.RouteId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.ConfigureAuditableProperties();
    }
}
