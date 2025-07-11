using BuildingBlocks.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SampleService.Domain.Entities;

namespace SampleService.Infrastructure.Data.Configurations;

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
        
        builder.ConfigureAuditableProperties();
        
        builder.Ignore(w => w.TemperatureF);
    }
}
