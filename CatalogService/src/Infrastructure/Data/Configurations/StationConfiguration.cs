using BuildingBlocks.Infrastructure.Data.Extensions;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

public class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
        builder.ToTable("Station");
        builder.ConfigureAuditableProperties();
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.StreetNumber)
            .HasMaxLength(50);

        builder.Property(s => s.Street)
            .HasMaxLength(200);

        builder.Property(s => s.Ward)
            .HasMaxLength(200);

        builder.Property(s => s.District)
            .HasMaxLength(200);

        builder.Property(s => s.City)
            .HasMaxLength(200);

        builder.Property(s => s.ThumbnailImageUrl)
            .HasMaxLength(500);
        builder.HasMany(s => s.StationRoutes)
            .WithOne(sr => sr.Station)
            .HasForeignKey(sr => sr.StationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Buses) 
            .WithOne(b => b.Station)
            .HasForeignKey(b => b.StationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
