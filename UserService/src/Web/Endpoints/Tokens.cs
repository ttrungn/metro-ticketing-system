using UserService.Application.WeatherForecasts.Queries.GetWeatherForecasts;

namespace UserService.Web.Endpoints;

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
