using BuildingBlocks.Infrastructure.Data.Extensions;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Ticket");
        builder.ConfigureAuditableProperties();
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(t => t.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
    }
}
