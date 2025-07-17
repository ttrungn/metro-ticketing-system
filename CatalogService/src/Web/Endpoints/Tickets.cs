
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Tickets.Commands.CreateTicket;
using CatalogService.Application.Tickets.Commands.DeleteTicket;
using CatalogService.Application.Tickets.Commands.UpdateTicket;
using CatalogService.Application.Tickets.Queries.GetSingleUseTicketWithPrice;
using Microsoft.AspNetCore.Mvc;
using CatalogService.Application.Tickets.Queries.GetActiveTicketByIdQuery;
using CatalogService.Application.Tickets.Queries.GetTickets;
using CatalogService.Domain.Enum;

namespace CatalogService.Web.Endpoints;


public class Tickets : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapGet(GetActiveTickets, "/")
            .MapPost(GetSingleUseTicketWithPrice, "/single-use-ticket-info/")
            .MapGet(GetActiveTicketsById, "/{id:guid}")
            .MapPost(CreateTicket, "/")
            .MapPut(UpdateTicket, "/")
            .MapDelete(DeleteTicket, "/{id:guid}")
            .MapGet(GetTickets, "/filter");
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

    private static async Task<IResult> CreateTicket(ISender sender, CreateTicketCommand command)
    {
        var response = await sender.Send(command);
        if (response.Succeeded)
        {
            return TypedResults.Created($"/api/catalog/tickets/{response.Data}", response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> UpdateTicket(ISender sender, UpdateTicketCommand command)
    {
        var response = await sender.Send(command);
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> DeleteTicket(ISender sender, [FromRoute] Guid id)
    {
        var command = new DeleteTicketCommand(id);
        var response = await sender.Send(command);
        if (response.Succeeded)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> GetTickets(
        ISender sender,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 8,
        [FromQuery] string? name = "",
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] TicketTypeEnum? ticketType = null,
        [FromQuery] bool? status = null)
    {
        var query = new GetTicketsQuery()
        {
            Page = page,
            PageSize = pageSize,
            Name = name ?? string.Empty,
            MinPrice = minPrice ?? Decimal.MinValue,
            MaxPrice = maxPrice ?? Decimal.MaxValue,
            TicketType = ticketType,
            Status = status
        };

        var response = await sender.Send(query);
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }
}
