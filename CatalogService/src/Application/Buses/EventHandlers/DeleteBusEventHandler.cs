using BuildingBlocks.Domain.Events.Buses;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Buses.EventHandlers;

public class DeleteBusEventHandler : INotificationHandler<DeleteBusEvent>
{
    private readonly ILogger<DeleteBusEventHandler> _logger;
    private readonly IMassTransitService<DeleteBusEvent> _massTransitService;

    public DeleteBusEventHandler(ILogger<DeleteBusEventHandler> logger, IMassTransitService<DeleteBusEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }

    public async Task Handle(DeleteBusEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending DeleteBusEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("DeleteBusEvent sent to Kafka");
    }
}
