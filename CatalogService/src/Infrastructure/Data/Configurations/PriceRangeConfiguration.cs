using BuildingBlocks.Infrastructure.Data.Extensions;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

public class PriceRangeConfiguration : IEntityTypeConfiguration<PriceRange>
{
    public void Configure(EntityTypeBuilder<PriceRange> builder)
    {
        builder.ToTable("PriceRange");
        builder.ConfigureAuditableProperties();
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FromKm)
            .IsRequired();

        builder.Property(p => p.ToKm)
            .IsRequired();

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
    }
}
