using CatalogService.Application.Routes.DTOs;
using CatalogService.Application.Stations.Commands.CreateStation;
using CatalogService.Application.Stations.Commands.DeleteStation;
using CatalogService.Application.Stations.Commands.UpdateStation;
using CatalogService.Application.Stations.Queries.GeAllActiveStationByName;
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
            .MapGet(GetStationById, "/{id:guid}")
                    .MapGet(GetAllActiveStationsByName, "/search/");

    }

    private static async Task<IResult> CreateStation(ISender sender, HttpRequest request)
    {
        var form = await request.ReadFormAsync();

        var thumbnailImage = form.Files.GetFile("thumbnailImage");
        Stream? thumbnailImageStream = null;
        string? thumbnailImageFileName = null;

        if (thumbnailImage is { Length: > 0 })
        {
            thumbnailImageStream = thumbnailImage.OpenReadStream();
            thumbnailImageFileName = thumbnailImage.FileName;
        }

        var command = new CreateStationCommand()
        {
            Name = form["name"].ToString(),
            StreetNumber = form["streetNumber"].ToString(),
            Street = form["street"].ToString(),
            Ward = form["ward"].ToString(),
            District = form["district"].ToString(),
            City = form["city"].ToString(),
            ThumbnailImageStream = thumbnailImageStream,
            ThumbnailImageFileName = thumbnailImageFileName
        };

        var response = await sender.Send(command);
        if (response.Succeeded)
        {
            return TypedResults.Created($"/api/catalog/stations/{response.Data}", response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> UpdateStation(ISender sender, HttpRequest request)

    {
        var form = await request.ReadFormAsync();

        var thumbnailImage = form.Files.GetFile("thumbnailImage");
        Stream? thumbnailImageStream = null;
        string? thumbnailImageFileName = null;
        if (thumbnailImage is { Length: > 0 })
        {
            thumbnailImageStream = thumbnailImage.OpenReadStream();
            thumbnailImageFileName = thumbnailImage.FileName;
        }

        var command = new UpdateStationCommand()
        {
            Id = Guid.TryParse(form["id"].ToString(), out var parsedId) ? parsedId : Guid.Empty,
            Name = form["name"].ToString(),
            StreetNumber = form["streetNumber"].ToString(),
            Street = form["street"].ToString(),
            Ward = form["ward"].ToString(),
            District = form["district"].ToString(),
            City = form["city"].ToString(),
            ThumbnailImageStream = thumbnailImageStream,
            ThumbnailImageFileName = thumbnailImageFileName
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

    private async Task<IResult> GetAllActiveStationsByName(ISender sender, [FromQuery] string? searchString)
    {
        var query = new GetAllAcitveStationsByNameQuery
        {
            Name = searchString ?? "",
        };

        var response = await sender.Send(query);

        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }
        return TypedResults.NotFound(response);
    }
}
