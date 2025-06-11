using CatalogService.Application.Buses.Commands.CreateBus;
using CatalogService.Application.Buses.Commands.DeleteBus;
using CatalogService.Application.Buses.Commands.UpdateBus;
using CatalogService.Application.Buses.Queries.GetBusById;
using CatalogService.Application.Buses.Queries.GetBuses;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Web.Endpoints;

public class Buses : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateBus, "/")
            .MapPut(UpdateBus, "/")
            .MapDelete(DeleteBus, "/{id:guid}")
            .MapGet(GetBusById, "/{id:guid}")
            .MapGet(GetBuses, "/");
    }

    private static async Task<IResult> CreateBus(ISender sender, [FromBody] CreateBusCommand command)
    {
        var response = await sender.Send(command);
        if (response.Succeeded)
        {
            return TypedResults.Created($"/api/catalog/buses/{response.Data}", response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> UpdateBus(ISender sender, [FromBody] UpdateBusCommand command)
    {
        var response = await sender.Send(command);
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> DeleteBus(ISender sender, [FromRoute] Guid id)
    {
        var command = new DeleteBusCommand(id);

        var response = await sender.Send(command);
        if (response.Succeeded)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound(response);
    }

    private static async Task<IResult> GetBusById(ISender sender, [FromRoute] Guid id)
    {
        var query = new GetBusByIdQuery(id);
        var response = await sender.Send(query);
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.NotFound(response);
    }

    private static async Task<IResult> GetBuses(
        ISender sender,
        [FromQuery] int page = 0,
        [FromQuery] Guid? stationId = null,
        [FromQuery] string? destinationName = "",
        [FromQuery] bool? status = false)
    {
        var query = new GetBusesQuery()
        {
            Page = page,
            StationId = stationId ?? Guid.Empty,
            DestinationName = destinationName ?? string.Empty,
            Status = status ?? false
        };
        var response = await sender.Send(query);
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }

}
