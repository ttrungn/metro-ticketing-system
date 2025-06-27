using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Ticket.DTO;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Routes.Queries.GetAllTicketsWithPriceQuery;

public record GetAllTicketsWithPriceQuery : IRequest<ServiceResponse<IEnumerable<TicketListResponseDto>>>
{
    public Guid? RouteId { get; init; }

    public Guid? EntryStationId { get; init; }

    public Guid? ExitStationId { get; init; }
}
public class GetAllTicketsWithPriceQueryValidator : AbstractValidator<GetAllTicketsWithPriceQuery>
{
    public GetAllTicketsWithPriceQueryValidator()
    {
    //     RuleFor(x => x.RouteId).NotEmpty().WithMessage("RouteId is required.");
    //     RuleFor(x => x.EntryStationId).NotEmpty().WithMessage("EntryStationId is required.");
    //     RuleFor(x => x.ExitStationId).NotEmpty().WithMessage("ExitStationId is required.");
    }
}

public class GetAllTicketsWithPriceQueryHandler : IRequestHandler<GetAllTicketsWithPriceQuery, 
                                            ServiceResponse<IEnumerable<TicketListResponseDto>>>
{
    private readonly ITicketService _ticketService;
    private readonly ILogger<GetAllTicketsWithPriceQueryHandler> _logger;

    public GetAllTicketsWithPriceQueryHandler(ITicketService ticketService, ILogger<GetAllTicketsWithPriceQueryHandler> logger)
    {
        _ticketService = ticketService;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<TicketListResponseDto>>> Handle(GetAllTicketsWithPriceQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var tickets = await _ticketService.GetAllActiveTicketAsync(cancellationToken);
            return new ServiceResponse<IEnumerable<TicketListResponseDto>>
            {
                Data = null,
                Message = "Get tickets successfully",
                Succeeded = true,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active tickets with price");
            return new ServiceResponse<IEnumerable<TicketListResponseDto>>
            {
                Data = null,
                Message = "Failed to get tickets",
                Succeeded = false
            };
        }
    }
}
