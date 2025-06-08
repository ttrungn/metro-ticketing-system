using CatalogService.Application.Stations.Commands.CreateStation;
using CatalogService.Application.Stations.Commands.DeleteStation;
using CatalogService.Application.Stations.Commands.UpdateStation;
using CatalogService.Application.Stations.Queries.GetStationById;
using CatalogService.Application.Stations.Queries.GetStations;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Web.Endpoints;

public class Stations : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapPost(CreateStation, "/")
            .MapPut(UpdateStation, "/")
            .MapDelete(DeleteStation, "/{id:guid}")
            .MapGet(GetStations, "/")
            .MapGet(GetStationById, "/{id:guid}");
    }

    private static async Task<IResult> CreateStation(
        ISender sender,
        [FromForm] IFormFile? thumbnailImageUrl,
        [FromQuery] string name,
        [FromQuery] string? streetNumber ,
        [FromQuery] string? street,
        [FromQuery] string? ward,
        [FromQuery] string? district,
        [FromQuery] string? city)
    {
        var command = new CreateStationCommand()
        {
            Name = name,
            StreetNumber = streetNumber,
            Street = street,
            Ward = ward,
            District = district,
            City = city,
            ThumbnailImageUrl = thumbnailImageUrl?.FileName ?? "Empty"
        };

        var response = await sender.Send(command);
        if (response.Succeeded)
        {
            return TypedResults.Created($"/api/catalog/stations/{response.Data}", response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> UpdateStation(
        ISender sender,
        [FromForm] IFormFile? thumbnailImageUrl,
        [FromQuery] Guid id,
        [FromQuery] string name,
        [FromQuery] string? streetNumber ,
        [FromQuery] string? street,
        [FromQuery] string? ward,
        [FromQuery] string? district,
        [FromQuery] string? city)
    {
        var command = new UpdateStationCommand()
        {
            Id = id,
            Name = name,
            StreetNumber = streetNumber,
            Street = street,
            Ward = ward,
            District = district,
            City = city,
            ThumbnailImageUrl = thumbnailImageUrl?.FileName ?? "Empty"
        };

        var response = await sender.Send(command);
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> DeleteStation(ISender sender, [FromRoute] Guid id)
    {
        var command = new DeleteStationCommand(id);

        var response = await sender.Send(command);
        if (response.Succeeded)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound(response);
    }

    private static async Task<IResult> GetStations(
        ISender sender,
        [FromQuery] int page = 0,
        [FromQuery] string? name = "",
        [FromQuery] bool? status = false)
    {
        var query = new GetStationsQuery()
        {
            Page = page,
            Name = name,
            Status = status
        };

        var response = await sender.Send(query);
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> GetStationById(ISender sender, [FromRoute] Guid id)
    {
        var query = new GetStationByIdQuery(id);

        var response = await sender.Send(query);
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.NotFound(response);
    }
}
