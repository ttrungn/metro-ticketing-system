using CatalogService.Domain.Entities;

namespace CatalogService.Application.Common.Interfaces.Services;

public interface IWeatherForecastService
{
    Task<Guid> CreateAsync(DateTime date, int temperatureC, string summary, CancellationToken cancellationToken = default);
    Task<IEnumerable<WeatherForecast>> GetAllAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, DateTime date, int temperatureC, string summary, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
