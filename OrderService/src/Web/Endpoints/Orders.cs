
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Orders.Queries.GetUserTicket;
using OrderService.Domain.Enums;

namespace OrderService.Web.Endpoints;

public class Orders : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetUserTicket, "/");
    }

    private static async Task<IResult> GetUserTicket([FromQuery] PurchaseTicketStatus status, ISender sender)
    {
        var response = await sender.Send(new GetUserTicketQuery(status));

        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }
}
