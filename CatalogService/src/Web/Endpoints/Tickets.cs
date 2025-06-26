
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Routes.Queries.GetAllTicketsWithPriceQuery;
using CatalogService.Application.Ticket.Queries.GetActiveTickets;
using CatalogService.Application.Tickets.Queries.GetActiveTicketByIdQuery;
using CatalogService.Application.Tickets.Queries.GetSingleUseTicketWithPrice;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Web.Endpoints;


public class Tickets : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapGet(GetActiveTickets, "/")
            .MapPost(GetSingleUseTicketWithPrice, "/single-use-ticket-info/")
            .MapGet(GetActiveTicketsById, "/{id:guid}");
    }

    private static async Task<IResult> GetActiveTickets(
        ITicketService ticketService,
        CancellationToken cancellationToken = default)
    {
        var tickets = await ticketService.GetAllActiveTicketAsync(cancellationToken);
        if (tickets == null || !tickets.Any())
        {
            return Results.NotFound("No active tickets found.");
        }
        return Results.Ok(tickets);
    }

    private static async Task<IResult> GetSingleUseTicketWithPrice(ISender sender, [FromBody] GetSingleUseTicketWithPriceQuery request)
    {
        var response = await sender.Send(request);


        if(response == null)
        {
            return TypedResults.BadRequest("Something is wrong");
        }
        return TypedResults.Ok(response);
    }

    private static async Task<IResult> GetActiveTicketsById(ISender sender, [FromRoute] Guid id)
    {
        var response = await sender.Send(new GetActiveTicketByIdQuery(id));
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.NotFound(response);
    }
}
