using CatalogService.Application.Routes.Commands.CreateRoute;
using CatalogService.Application.Routes.Commands.DeleteRoute;
using CatalogService.Application.Routes.Commands.UpdateRoute;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Web.Endpoints;

public class Routes : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateRoute, "/")
            .MapPut(UpdateRoute, "/")
            .MapDelete(DeleteRoute, "/{id:guid}");

    }

    private static async Task<IResult> CreateRoute(ISender sender, [FromBody] CreateRouteCommand command)
    {
        var result = await sender.Send(command);

        if (result.Succeeded)
        {
            return TypedResults.Created($"/api/catalog/routes/{result.Data}", result);
        }

        return TypedResults.BadRequest(result);
    }

    private static async Task<IResult> UpdateRoute(ISender sender, [FromBody] UpdateRouteCommand command)
    {
        var result = await sender.Send(command);

        if (result.Succeeded)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }

    private static async Task<IResult> DeleteRoute(ISender sender, [FromRoute] Guid id)
    {
        var command = new DeleteRouteCommand(id);

        var result = await sender.Send(command);

        if (result.Succeeded)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.BadRequest(result);
    }
}
