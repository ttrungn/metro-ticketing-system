using OrderService.Application.WeatherForecasts.Commands.CreateWeatherForecast;
using OrderService.Application.WeatherForecasts.Commands.DeleteWeatherForecast;
using OrderService.Application.WeatherForecasts.Commands.UpdateWeatherForecast;
using OrderService.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using OrderService.Domain.Entities;

namespace OrderService.Web.Endpoints;

public class WeatherForecasts : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetWeatherForecasts, "/")
            .MapPost(CreateWeatherForecast, "/")
            .MapPut(UpdateWeatherForecast, "/{id:guid}")
            .MapDelete(DeleteWeatherForecast, "/{id:guid}");
    }

    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecasts(ISender sender)
    {
        return await sender.Send(new GetWeatherForecastsQuery());
    }

    public async Task<IResult> CreateWeatherForecast(ISender sender, CreateWeatherForecastCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/weather-forecasts/{id}");
    }

    public async Task<IResult> UpdateWeatherForecast(ISender sender, Guid id, UpdateWeatherForecastCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();
        await sender.Send(command);
        return TypedResults.NoContent();
    }

    public async Task<IResult> DeleteWeatherForecast(ISender sender, Guid id)
    {
        await sender.Send(new DeleteWeatherForecastCommand(id));
        return TypedResults.NoContent();
    }
}
