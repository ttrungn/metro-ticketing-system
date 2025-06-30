using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Carts.Commands.AddToCart;
using OrderService.Application.Carts.Commands.DeleteCart;
using OrderService.Application.Carts.Commands.UpdateCart;
using OrderService.Application.Carts.Queries;

namespace OrderService.Web.Endpoints;

public class Cart : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(AddToCartAsync, "/")
            .MapGet(GetCartAsync, "/")
            .MapPut(UpdateQuantityCart, "/")
            .MapDelete(DeleteCart, "/{id:guid}")
            .MapGet(GetQuantitiesCartAsync, "/quantities");
    }

    public static async Task<IResult> AddToCartAsync([FromBody] AddToCartCommand command, ISender sender)
    {
        var response = await sender.Send(command);

        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }
    public static async Task<IResult> GetCartAsync(CancellationToken cancellationToken, ISender sender)
    {
        var response = await sender.Send(new GetCartQuery(), cancellationToken);

        if (response.Succeeded)
        {
            decimal totalPrice = response.Data!.Sum(i => i.Price * i.Quantity);

            return Results.Ok(new
            {
                succeeded = true,
                message = "Lấy giỏ hàng thành công.",
                data = response.Data,
                totalPrice = totalPrice
            });
        }

        return TypedResults.BadRequest(response);
    }
    public static async Task<IResult> GetQuantitiesCartAsync(CancellationToken cancellationToken, ISender sender)
    {
        var response = await sender.Send(new GetQuantitiesCartQuery(), cancellationToken);

        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }
    public static async Task<IResult> UpdateQuantityCart([FromBody] UpdateCartCommand request, ISender sender)
    {
        var response = await sender.Send(request);

        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }
    public static async Task<IResult> DeleteCart([FromRoute] Guid id, ISender sender)
    {
        var command = new DeleteCartCommand(id);

        var response = await sender.Send(command);

        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }
}
