
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.MomoPayment.Commands.ConfirmMomoPayment;
using OrderService.Application.MomoPayment.Commands.CreateMomoPayment;

namespace OrderService.Web.Endpoints;


public class Payment : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapPost(MomoPayment, "/momo/create")
            .MapPost(MomoPaymentConfirm, "/momo/confirm");
    }

    public static async Task<IResult> MomoPayment([FromBody] CreateMomoPaymentCommand command,
        ISender sender
        )
    {
        var response = await sender.Send(command);

        if (response != null)
        {
            return TypedResults.Ok(response);
        }
        else return TypedResults.BadRequest();
    }

    public static async Task<IResult> MomoPaymentConfirm([FromBody] ConfirmMomoPaymentCommand command,
        ISender sender
        )
    {
        var response = await sender.Send(command);
        if (response != null)
        {
            return TypedResults.Ok(response);
        }
        else return TypedResults.BadRequest();
    }
    
   
}
