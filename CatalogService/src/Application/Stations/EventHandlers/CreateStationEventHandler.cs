using BuildingBlocks.Domain.Events.Stations;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Stations.EventHandlers;

public class CreateStationEventHandler : INotificationHandler<CreateStationEvent>
{
    private readonly ILogger<CreateStationEventHandler> _logger;
    private readonly IMassTransitService<CreateStationEvent> _massTransitService;

    public CreateStationEventHandler(ILogger<CreateStationEventHandler> logger, IMassTransitService<CreateStationEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }

    public async Task Handle(CreateStationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending CreateStationEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("CreateStationEvent sent to Kafka");
    }
}
