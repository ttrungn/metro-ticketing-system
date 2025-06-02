using OrderService.Domain.Entities;

namespace OrderService.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<WeatherForecast> WeatherForecasts { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
