using BuildingBlocks.Domain.Events.Buses;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Buses.EventHandlers;

public class UpdateBusEventHandler : INotificationHandler<UpdateBusEvent>
{
    private readonly ILogger<UpdateBusEventHandler> _logger;
    private readonly IMassTransitService<UpdateBusEvent> _massTransitService;

    public UpdateBusEventHandler(IMassTransitService<UpdateBusEvent> massTransitService, ILogger<UpdateBusEventHandler> logger)
    {
        _massTransitService = massTransitService;
        _logger = logger;
    }

    public async Task Handle(UpdateBusEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending UpdateBusEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("UpdateBusEvent sent to Kafka");
    }
}
