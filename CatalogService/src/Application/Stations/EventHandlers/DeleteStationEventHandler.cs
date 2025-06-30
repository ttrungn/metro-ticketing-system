using BuildingBlocks.Domain.Events.Stations;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Stations.EventHandlers;

public class DeleteStationEventHandler : INotificationHandler<DeleteStationEvent>
{
    private readonly ILogger<DeleteStationEventHandler> _logger;
    private readonly IMassTransitService<DeleteStationEvent> _massTransitService;

    public DeleteStationEventHandler(ILogger<DeleteStationEventHandler> logger, IMassTransitService<DeleteStationEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }

    public async Task Handle(DeleteStationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending DeleteStationEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("DeleteStationEvent sent to Kafka");
    }
}
