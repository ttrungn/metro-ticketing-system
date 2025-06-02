using Microsoft.Extensions.Logging;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Domain.Entities;

namespace CatalogService.Application.WeatherForecasts.Queries.GetWeatherForecasts;

public record GetWeatherForecastsQuery : IRequest<IEnumerable<WeatherForecast>>;

public class GetWeatherForecastsQueryHandler : IRequestHandler<GetWeatherForecastsQuery, IEnumerable<WeatherForecast>>
{
    private readonly IWeatherForecastService _weatherForecastService;
    private readonly ILogger<GetWeatherForecastsQueryHandler> _logger;

    public GetWeatherForecastsQueryHandler(
        IWeatherForecastService weatherForecastService,
        ILogger<GetWeatherForecastsQueryHandler> logger)
    {
        _weatherForecastService = weatherForecastService;
        _logger = logger;
    }

    public async Task<IEnumerable<WeatherForecast>> Handle(GetWeatherForecastsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all weather forecasts");
        var forecasts = await _weatherForecastService.GetAllAsync(cancellationToken);
        _logger.LogInformation("Retrieved {Count} weather forecasts", forecasts.Count());
        return forecasts;
    }
}
