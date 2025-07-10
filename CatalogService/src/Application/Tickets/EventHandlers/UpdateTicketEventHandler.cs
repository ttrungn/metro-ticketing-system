using BuildingBlocks.Domain.Events.Tickets;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Tickets.EventHandlers;

public class UpdateTicketEventHandler : INotificationHandler<UpdateTicketEvent>
{
    private readonly ILogger<UpdateTicketEventHandler> _logger;
    private readonly IMassTransitService<UpdateTicketEvent> _massTransitService;

    public UpdateTicketEventHandler(IMassTransitService<UpdateTicketEvent> massTransitService, ILogger<UpdateTicketEventHandler> logger)
    {
        _massTransitService = massTransitService;
        _logger = logger;
    }

    public async Task Handle(UpdateTicketEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending UpdateTicketEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("UpdateTicketEvent sent to Kafka");
    }
}
