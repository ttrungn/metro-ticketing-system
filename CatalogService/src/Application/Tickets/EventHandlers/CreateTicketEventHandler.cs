using BuildingBlocks.Domain.Events.Tickets;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Tickets.EventHandlers;

public class CreateTicketEventHandler : INotificationHandler<CreateTicketEvent>
{
    private readonly ILogger<CreateTicketEventHandler> _logger;
    private readonly IMassTransitService<CreateTicketEvent> _massTransitService;

    public CreateTicketEventHandler(ILogger<CreateTicketEventHandler> logger, IMassTransitService<CreateTicketEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }

    public async Task Handle(CreateTicketEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending CreateTicketEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("CreateTicketEvent sent to Kafka");
    }
}
