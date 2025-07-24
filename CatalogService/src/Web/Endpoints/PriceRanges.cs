using CatalogService.Application.PriceRanges.Commands;
using CatalogService.Application.PriceRanges.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Web.Endpoints;

public class PriceRanges : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetPriceRanges, "/")
            .MapGet(GetPriceRangeById, "/{id:guid}")
            .MapPost(CreatePriceRange, "/")
            .MapPut(UpdatePriceRange, "/")
            .MapDelete(DeletePriceRange, "/{id:guid}");
    }

    private static async Task<IResult> GetPriceRanges(
        ISender sender,
        int page = 0,
        int pageSize = 8,
        bool? deleteFlag = null)
    {
        var query = new GetPriceRangesQuery() { Page = page, PageSize = pageSize, DeleteFlag = deleteFlag };

        var response = await sender.Send(query);
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> CreatePriceRange(
        ISender sender,
        [FromBody] CreatePriceRangeCommand command)
    {
        var response = await sender.Send(command);
        if (response.Succeeded)
        {
            return TypedResults.Created($"/api/catalog/PriceRanges/{response.Data}", response);
        }

        return TypedResults.BadRequest(response);
    }
    private static async Task<IResult> UpdatePriceRange(
        ISender sender,
        [FromBody] UpdatePriceRangeCommand command)
    {
        var response = await sender.Send(command);
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> DeletePriceRange(
        ISender sender,
        [FromRoute] Guid id)
    {
        var command = new DeletePriceRangeCommand(id);

        var response = await sender.Send(command);
        if (response.Succeeded)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound(response);
    }

    private static async Task<IResult> GetPriceRangeById(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetPriceRangeByIdQuery(id);

        var response = await sender.Send(query);
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.NotFound(response);
    }
}
