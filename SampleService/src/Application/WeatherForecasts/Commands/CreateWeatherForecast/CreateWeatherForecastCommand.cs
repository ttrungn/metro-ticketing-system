using BuildingBlocks.Constants;
using BuildingBlocks.Domain.Constants;
using Microsoft.Extensions.Logging;
using SampleService.Application.Common.Interfaces.Services;
using SampleService.Application.Common.Security;

namespace SampleService.Application.WeatherForecasts.Commands.CreateWeatherForecast;

// [Authorize(Roles = Roles.Administrator)]
public record CreateWeatherForecastCommand : IRequest<Guid>
{
    public Stream FileStream { get; init; } = null!;
    public required DateTime Date { get; init; }
    public required int TemperatureC { get; init; }
    public required string Summary { get; init; }
}

public class CreateWeatherForecastCommandValidator : AbstractValidator<CreateWeatherForecastCommand>
{
    public CreateWeatherForecastCommandValidator()
    {
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

public class CreateWeatherForecastCommandHandler : IRequestHandler<CreateWeatherForecastCommand, Guid>
{
    private readonly IWeatherForecastService _weatherForecastService;
    private readonly ILogger<CreateWeatherForecastCommandHandler> _logger;

    public CreateWeatherForecastCommandHandler(
        IWeatherForecastService weatherForecastService,
        ILogger<CreateWeatherForecastCommandHandler> logger)
    {
        _weatherForecastService = weatherForecastService;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateWeatherForecastCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating weather forecast for date: {Date}", request.Date);
        var id = await _weatherForecastService.CreateAsync(
            request.Date,
            request.TemperatureC,
            request.Summary,
            cancellationToken);
        _logger.LogInformation("Created weather forecast with ID: {Id}", id);
        return id;
    }
}
