using BuildingBlocks.Constants;
using BuildingBlocks.Domain.Constants;
using Microsoft.Extensions.Logging;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Common.Security;

namespace CatalogService.Application.WeatherForecasts.Commands.DeleteWeatherForecast;

[Authorize(Roles = Roles.Administrator)]
public record DeleteWeatherForecastCommand(Guid Id) : IRequest;

public class DeleteWeatherForecastCommandHandler : IRequestHandler<DeleteWeatherForecastCommand>
{
    private readonly IWeatherForecastService _weatherForecastService;
    private readonly ILogger<DeleteWeatherForecastCommandHandler> _logger;

    public DeleteWeatherForecastCommandHandler(
        IWeatherForecastService weatherForecastService,
        ILogger<DeleteWeatherForecastCommandHandler> logger)
    {
        _weatherForecastService = weatherForecastService;
        _logger = logger;
    }

    public async Task Handle(DeleteWeatherForecastCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting weather forecast: {Id}", request.Id);
        await _weatherForecastService.DeleteAsync(request.Id, cancellationToken);
        _logger.LogInformation("Deleted weather forecast: {Id}", request.Id);
    }
}
