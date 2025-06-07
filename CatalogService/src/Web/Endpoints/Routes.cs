using CatalogService.Application.Routes.Commands.CreateRoute;
using CatalogService.Application.Routes.Commands.DeleteRoute;
using CatalogService.Application.Routes.Commands.UpdateRoute;
using CatalogService.Application.Routes.Queries.GetRouteById;
using CatalogService.Application.Routes.Queries.GetRoutes;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Web.Endpoints;

public class Routes : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapPost(CreateRoute, "/")
            .MapPut(UpdateRoute, "/")
            .MapDelete(DeleteRoute, "/{id:guid}")
            .MapGet(GetRouteById, "/{id:guid}")
            .MapGet(GetRoutes, "/");
    }

    private static async Task<IResult> CreateRoute(
        ISender sender,
        [FromForm] IFormFile? thumbnailImageUrl,
        [FromQuery] string code,
        [FromQuery] string name,
        [FromQuery] double lengthInKm
        )
    {
        var command = new CreateRouteCommand()
        {
            Code = code,
            Name = name,
            ThumbnailImageUrl = thumbnailImageUrl?.FileName ?? "Empty",
            LengthInKm = lengthInKm
        };

        var response = await sender.Send(command);

        if (response.Succeeded)
        {
            return TypedResults.Created($"/api/catalog/routes/{response.Data}", response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> UpdateRoute(
        ISender sender,
        [FromForm] IFormFile? file,
        [FromQuery] Guid id,
        [FromQuery] string code,
        [FromQuery] string name,
        [FromQuery] double lengthInKm)
    {
        var command = new UpdateRouteCommand()
        {
            Id = id,
            Code = code,
            Name = name,
            ThumbnailImageUrl = file?.FileName ?? "Empty",
            LengthInKm = lengthInKm
        };

        var response = await sender.Send(command);
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> DeleteRoute(ISender sender, [FromRoute] Guid id)
    {
        var command = new DeleteRouteCommand(id);

        var response = await sender.Send(command);
        if (response.Succeeded)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound(response);
    }

    private static async Task<IResult> GetRoutes(
        ISender sender,
        [FromQuery] int page = 0,
        [FromQuery] string? name = "")
    {
        var query = new GetRoutesQuery
        {
            Page = page,
            Name = name
        };

        var response = await sender.Send(query);
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> GetRouteById(ISender sender, [FromRoute] Guid id)
    {
        var query = new GetRouteByIdQuery(id);

        var response = await sender.Send(query);
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.NotFound(response);
    }

}
