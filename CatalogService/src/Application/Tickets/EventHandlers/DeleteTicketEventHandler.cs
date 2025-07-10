using BuildingBlocks.Domain.Events.Tickets;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Tickets.EventHandlers;

public class DeleteTicketEventHandler : INotificationHandler<DeleteTicketEvent>
{
    private readonly ILogger<DeleteTicketEventHandler> _logger;
    private readonly IMassTransitService<DeleteTicketEvent> _massTransitService;

    public DeleteTicketEventHandler(ILogger<DeleteTicketEventHandler> logger, IMassTransitService<DeleteTicketEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }

    public async Task Handle(DeleteTicketEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending DeleteTicketEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("DeleteTicketEvent sent to Kafka");
    }
}
