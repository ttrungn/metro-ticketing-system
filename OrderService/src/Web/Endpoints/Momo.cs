
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.MomoPayment.Commands.CreateMomoPayment;

namespace OrderService.Web.Endpoints;


public class Momo : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapPost(MomoPayment, "/create");
    }

    public static async Task<IResult> MomoPayment([FromBody] CreateMomoPaymentCommand command,
        ISender sender
        )
    {
        var response = await sender.Send(command);
        return TypedResults.Ok(response);
    }
}
