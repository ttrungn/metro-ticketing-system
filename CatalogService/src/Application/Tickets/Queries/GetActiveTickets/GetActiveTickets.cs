using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Ticket.DTO;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Ticket.Queries.GetActiveTickets;

public record GetActiveTicketsQuery : IRequest<ServiceResponse<TicketListResponseDto>>
{
}

public class GetActiveTicketsQueryValidator : AbstractValidator<GetActiveTicketsQuery>
{
    public GetActiveTicketsQueryValidator()
    {
    }
}

public class GetActiveTicketsQueryHandler : IRequestHandler<GetActiveTicketsQuery, ServiceResponse<TicketListResponseDto>>
{
    private readonly ITicketService _service;
    private readonly ILogger<GetActiveTicketsQueryHandler> _logger;

    public GetActiveTicketsQueryHandler(ITicketService service, ILogger<GetActiveTicketsQueryHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<ServiceResponse<TicketListResponseDto>> Handle(GetActiveTicketsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var activeTickets = await _service.GetAllActiveTicketAsync(cancellationToken);
            var response = new TicketListResponseDto
            {
                Tickets = activeTickets.ToList()  // assuming it returns 
            };

            return new ServiceResponse<TicketListResponseDto>
            {
                Data = response,
                Message = "Get tickets successfully",
                Succeeded = true,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active tickets");
            return new ServiceResponse<TicketListResponseDto>
            {
                Data = null,
                Message = "Failed to get tickets",
                Succeeded = false
            };
        }
    }
}
