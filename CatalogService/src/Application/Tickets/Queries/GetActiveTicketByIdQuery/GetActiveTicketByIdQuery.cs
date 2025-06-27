using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Tickets.DTO;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Tickets.Queries.GetActiveTicketByIdQuery;

public record GetActiveTicketByIdQuery(Guid Id) : IRequest<ServiceResponse<TicketDto>>;

public class GetActiveTicketByIdQueryHandler : IRequestHandler<GetActiveTicketByIdQuery, ServiceResponse<TicketDto>>
{
    private readonly ITicketService _ticketService;
    private readonly ILogger<GetActiveTicketByIdQueryHandler> _logger;

    public GetActiveTicketByIdQueryHandler(
        ITicketService ticketService,
        ILogger<GetActiveTicketByIdQueryHandler> logger)
    {
        _ticketService = ticketService;
        _logger = logger;
    }

    public async Task<ServiceResponse<TicketDto>> Handle(GetActiveTicketByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetActiveTicketByIdQuery for TicketId: {TicketId}", request.Id);

        var ticket = await _ticketService.GetByIdAsync(request.Id, cancellationToken);

        if (ticket == null)
        {
            return new ServiceResponse<TicketDto>
            {
                Succeeded = false,
                Message = "Ticket not found.",
                Data = null
            };
        }

        return new ServiceResponse<TicketDto>
        {
            Message = "Get Active Ticket By Id Successfully",
            Succeeded = true,
            Data = ticket,
        };
    }
}

