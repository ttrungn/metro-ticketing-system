using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Orders.Commands.UpdateTicketToUnusedOrExpired;
using OrderService.Application.Orders.Commands.UpdateTicketToUsed;
using OrderService.Application.Orders.Queries.GetUserTicket;
using OrderService.Domain.Enums;

namespace OrderService.Web.Endpoints;

public class Orders : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetUserTicket, "/")
            .MapPut(UpdateTicketToUsed, "/update-ticket-to-used")
            .MapPut(UpdateTicketToUnusedOrExpired, "/update-ticket-to-unused-or-expired");
    }

    private static async Task<IResult> GetUserTicket([FromQuery] PurchaseTicketStatus status,
        ISender sender)
    {
        var response = await sender.Send(new GetUserTicketQuery(status));

        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> UpdateTicketToUsed(
        [FromBody] UpdateTicketToUsedCommand command,
        ISender sender)
    {
        var response = await sender.Send(command);

        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> UpdateTicketToUnusedOrExpired(
        [FromBody] UpdateTicketToUnusedOrExpiredCommand command,
        ISender sender)
    {
        var response = await sender.Send(command);

        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }

}
