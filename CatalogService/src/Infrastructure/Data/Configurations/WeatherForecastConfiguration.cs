using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CatalogService.Domain.Entities;

namespace CatalogService.Infrastructure.Data.Configurations;

public class WeatherForecastConfiguration : IEntityTypeConfiguration<WeatherForecast>
{
    public void Configure(EntityTypeBuilder<WeatherForecast> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Date)
            .IsRequired();

        builder.Property(w => w.TemperatureC)
            .IsRequired();

        builder.Property(w => w.Summary)
            .IsRequired()
            .HasMaxLength(200);

        // Ignore computed property
        builder.Ignore(w => w.TemperatureF);
    }
}
