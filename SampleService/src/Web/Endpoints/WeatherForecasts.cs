using SampleService.Application.WeatherForecasts.Commands.CreateWeatherForecast;
using SampleService.Application.WeatherForecasts.Commands.DeleteWeatherForecast;
using SampleService.Application.WeatherForecasts.Commands.UpdateWeatherForecast;
using SampleService.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using SampleService.Domain.Entities;

namespace SampleService.Web.Endpoints;

public class WeatherForecasts : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapGet(GetWeatherForecasts, "/")
            .MapPost(CreateWeatherForecast, "/")
            .MapPut(UpdateWeatherForecast, "/{id:guid}")
            .MapDelete(DeleteWeatherForecast, "/{id:guid}");
    }

    private async Task<IEnumerable<WeatherForecast>> GetWeatherForecasts(ISender sender)
    {
        return await sender.Send(new GetWeatherForecastsQuery());
    }

    private async Task<IResult> CreateWeatherForecast(ISender sender, CreateWeatherForecastCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/weather-forecasts/{id}");
    }

    private async Task<IResult> UpdateWeatherForecast(ISender sender, Guid id, UpdateWeatherForecastCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();
        await sender.Send(command);
        return TypedResults.NoContent();
    }

    private async Task<IResult> DeleteWeatherForecast(ISender sender, Guid id)
    {
        await sender.Send(new DeleteWeatherForecastCommand(id));
        return TypedResults.NoContent();
    }
}
