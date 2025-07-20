using Microsoft.Extensions.Logging;
using NotificationService.Application.Common.Interfaces.Services;
using NotificationService.Application.Mails.Queries.SendWelcome;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.WeatherForecasts.Queries.GetWeatherForecasts;

public record GetWeatherForecastsQuery : IRequest<IEnumerable<WeatherForecast>>;

public class GetWeatherForecastsQueryHandler : IRequestHandler<GetWeatherForecastsQuery, IEnumerable<WeatherForecast>>
{
    private readonly IWeatherForecastService _weatherForecastService;
    private readonly ILogger<GetWeatherForecastsQueryHandler> _logger;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IEmailService _emailService;
    
    public GetWeatherForecastsQueryHandler(
        IWeatherForecastService weatherForecastService,
        ILogger<GetWeatherForecastsQueryHandler> logger,
        IEmailTemplateService emailTemplateService,
        IEmailService emailService
        )
    {
        _weatherForecastService = weatherForecastService;
        _logger = logger;
        _emailTemplateService = emailTemplateService;
        _emailService = emailService;
    }

    public async Task<IEnumerable<WeatherForecast>> Handle(GetWeatherForecastsQuery request, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync(new MailData()
        {
            EmailBody = await _emailTemplateService.GenerateWelcomeTemplate("Trung", "Nguyen"),
            EmailSubject = "Welcome to Metro Ticketing System",
            EmailToId = "trungnguyen0803forwork@gmail.com",
            EmailToName = "Trung Nguyen"
        });
        return new List<WeatherForecast>();
    }
}
