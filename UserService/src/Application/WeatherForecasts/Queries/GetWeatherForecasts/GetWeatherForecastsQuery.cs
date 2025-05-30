using UserService.Application.Common.Interfaces.Repositories;
using BuildingBlocks.Constants;
using BuildingBlocks.Domain.Constants;

namespace UserService.Application.WeatherForecasts.Queries.GetWeatherForecasts;

public record GetWeatherForecastsQuery : IRequest<string>;

public class GetWeatherForecastsQueryHandler : IRequestHandler<GetWeatherForecastsQuery, string>
{
    private readonly ITokenRepository _tokenRepository;
    public GetWeatherForecastsQueryHandler(ITokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }
    public Task<string> Handle(GetWeatherForecastsQuery request, CancellationToken cancellationToken)
    {
        var token = _tokenRepository.GenerateJwtToken(
            "7676b67a-f3c3-425c-9902-6a6417ce6994",
            "administrator@localhost",
            [Roles.Administrator]
        );
    
        return Task.FromResult(token);
    }
}
