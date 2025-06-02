using BuildingBlocks.Constants;
using BuildingBlocks.Domain.Constants;
using Microsoft.Extensions.Logging;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Common.Security;
using CatalogService.Domain.Entities;

namespace CatalogService.Application.WeatherForecasts.Commands.UpdateWeatherForecast;

[Authorize(Roles = Roles.Administrator)]
public record UpdateWeatherForecastCommand : IRequest
{
    public required Guid Id { get; init; }
    public required DateTime Date { get; init; }
    public required int TemperatureC { get; init; }
    public required string Summary { get; init; }
}

public class UpdateWeatherForecastCommandValidator : AbstractValidator<UpdateWeatherForecastCommand>
{
    public UpdateWeatherForecastCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.")
            .GreaterThan(DateTime.MinValue).WithMessage("Date must be valid.");

        RuleFor(x => x.TemperatureC)
            .InclusiveBetween(-100, 100).WithMessage("Temperature must be between -100 and 100 degrees Celsius.");

        RuleFor(x => x.Summary)
            .NotEmpty().WithMessage("Summary is required.")
            .MaximumLength(250).WithMessage("Summary cannot exceed 250 characters.");
    }
}

public class UpdateWeatherForecastCommandHandler : IRequestHandler<UpdateWeatherForecastCommand>
{
    private readonly IWeatherForecastService _weatherForecastService;
    private readonly ILogger<UpdateWeatherForecastCommandHandler> _logger;

    public UpdateWeatherForecastCommandHandler(
        IWeatherForecastService weatherForecastService,
        ILogger<UpdateWeatherForecastCommandHandler> logger)
    {
        _weatherForecastService = weatherForecastService;
        _logger = logger;
    }

    public async Task Handle(UpdateWeatherForecastCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating weather forecast: {Id}", request.Id);
        await _weatherForecastService.UpdateAsync(
            request.Id,
            request.Date,
            request.TemperatureC,
            request.Summary,
            cancellationToken);
        _logger.LogInformation("Updated weather forecast: {Id}", request.Id);
    }
}
