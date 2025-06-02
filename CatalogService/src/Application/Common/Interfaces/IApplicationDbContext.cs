using CatalogService.Domain.Entities;

namespace CatalogService.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<WeatherForecast> WeatherForecasts { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
