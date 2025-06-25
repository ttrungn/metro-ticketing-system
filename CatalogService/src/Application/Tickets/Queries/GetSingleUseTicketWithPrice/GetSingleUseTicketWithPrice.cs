using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Tickets.DTO;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Tickets.Queries.GetSingleUseTicketWithPrice;

public record GetSingleUseTicketWithPriceQuery : IRequest<ServiceResponse<SingleUseTicketResponseDto>>
{
    public Guid RouteId { get; init; }

    public Guid EntryStationId { get; init; }

    public Guid ExitStationId { get; init; }
}

public class GetSingleUseTicketWithPriceQueryValidator : AbstractValidator<GetSingleUseTicketWithPriceQuery>
{
    public GetSingleUseTicketWithPriceQueryValidator()
    {
    }
}

public class GetSingleUseTicketWithPriceQueryHandler : IRequestHandler<GetSingleUseTicketWithPriceQuery, ServiceResponse<SingleUseTicketResponseDto>>
{
    private readonly ITicketService _ticketService;
    private readonly ILogger<GetSingleUseTicketWithPriceQueryHandler> _logger;

    public GetSingleUseTicketWithPriceQueryHandler(
        ITicketService ticketService,
        ILogger<GetSingleUseTicketWithPriceQueryHandler> logger)
    {
        _ticketService = ticketService;
        _logger = logger;
    }

    public async Task<ServiceResponse<SingleUseTicketResponseDto>> Handle(GetSingleUseTicketWithPriceQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetSingleUseTicketWithPriceQuery for RouteId: {RouteId}, EntryStationId: {EntryStationId}, ExitStationId: {ExitStationId}",
            request.RouteId, request.EntryStationId, request.ExitStationId);

        var ticketInfo = await _ticketService.GetSingleUseTicketInfo(request, cancellationToken);

        return new ServiceResponse<SingleUseTicketResponseDto>()
        {
            Data = ticketInfo,
            Message = "Get Single Use Ticket Price Successully",
            Succeeded = true,
        };
    }
}

