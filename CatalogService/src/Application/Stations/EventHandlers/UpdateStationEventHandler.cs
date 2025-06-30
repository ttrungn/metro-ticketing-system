using BuildingBlocks.Domain.Events.Stations;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Stations.EventHandlers;

public class UpdateStationEventHandler : INotificationHandler<UpdateStationEvent>
{
    private readonly ILogger<UpdateStationEventHandler> _logger;
    private readonly IMassTransitService<UpdateStationEvent> _massTransitService;

    public UpdateStationEventHandler(ILogger<UpdateStationEventHandler> logger, IMassTransitService<UpdateStationEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }

    public async Task Handle(UpdateStationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending UpdateStationEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("UpdateStationEvent sent to Kafka");
    }
}
