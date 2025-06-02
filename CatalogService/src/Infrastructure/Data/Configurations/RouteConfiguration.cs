using BuildingBlocks.Infrastructure.Data.Extensions;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

public class RouteConfiguration : IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> builder)
    {
        builder.ConfigureAuditableProperties();
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Code).IsRequired().HasMaxLength(50);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(200);
        builder.Property(r => r.ThumbnailImageUrl).HasMaxLength(500);
        builder.Property(r => r.LengthInKm).IsRequired();
        builder.HasMany(r => r.StationRoutes)
            .WithOne(sr => sr.Route)
            .HasForeignKey(sr => sr.RouteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Line)
            .WithOne(l => l.Route)
            .HasForeignKey<Line>(l => l.RouteId);
    }
}
