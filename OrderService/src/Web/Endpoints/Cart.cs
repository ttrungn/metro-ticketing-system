using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Carts.Commands.AddToCart;
using OrderService.Application.Carts.Queries;

namespace OrderService.Web.Endpoints;

public class Cart : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(AddToCartAsync, "/")
            .MapGet(GetCartAsync, "/");
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
}
