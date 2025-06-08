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

    private static async Task<IResult> CreateRoute(ISender sender, HttpRequest request)
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

        var command = new CreateRouteCommand()
        {
            Name = form["name"].ToString(),
            ThumbnailImageStream = thumbnailImageStream,
            ThumbnailImageFileName = thumbnailImageFileName,
            LengthInKm = double.TryParse(form["lengthInKm"], out var parsedLength) ? parsedLength : 0.0
        };

        var response = await sender.Send(command);
        if (response.Succeeded)
        {
            return TypedResults.Created($"/api/catalog/routes/{response.Data}", response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> UpdateRoute(ISender sender, HttpRequest request)
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

        var command = new UpdateRouteCommand()
        {
            Id = Guid.TryParse(form["id"].ToString(), out var parsedId) ? parsedId : Guid.Empty,
            Name = form["name"].ToString(),
            ThumbnailImageStream = thumbnailImageStream,
            ThumbnailImageFileName = thumbnailImageFileName,
            LengthInKm = double.TryParse(form["lengthInKm"], out var parsedLength) ? parsedLength : 0.1
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
