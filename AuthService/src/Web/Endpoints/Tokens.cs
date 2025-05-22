using AuthService.Application.WeatherForecasts.Queries.GetWeatherForecasts;

namespace AuthService.Web.Endpoints;

public class Tokens : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetJwtToken);
    }

    private static async Task<string> GetJwtToken(ISender sender)
    {
        return await sender.Send(new GetWeatherForecastsQuery());
    }
}
