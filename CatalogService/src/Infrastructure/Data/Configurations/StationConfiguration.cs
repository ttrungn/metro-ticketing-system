using BuildingBlocks.Infrastructure.Data.Extensions;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

public class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
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

        builder.HasMany(s => s.FromSegments)
            .WithOne(ls => ls.FromStation)
            .HasForeignKey(ls => ls.FromStationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.ToSegments)
            .WithOne(ls => ls.ToStation)
            .HasForeignKey(ls => ls.ToStationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.EntryLines)
            .WithOne(l => l.EntryStation)
            .HasForeignKey(l => l.EntryStationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.ExitLines)
            .WithOne(l => l.ExitStation)
            .HasForeignKey(l => l.ExitStationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
