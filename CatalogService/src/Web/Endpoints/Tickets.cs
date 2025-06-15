
using CatalogService.Application.Common.Interfaces.Services;

namespace CatalogService.Web.Endpoints;


public class Tickets : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapGet("/", GetActiveTickets);
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
}
