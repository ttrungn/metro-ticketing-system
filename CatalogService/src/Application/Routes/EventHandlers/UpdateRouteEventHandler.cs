using BuildingBlocks.Domain.Events.Routes;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Routes.EventHandlers;

public class UpdateRouteEventHandler : INotificationHandler<UpdateRouteEvent>
{
    private readonly ILogger<UpdateRouteEventHandler> _logger;
    private readonly IMassTransitService<UpdateRouteEvent> _massTransitService;

    public UpdateRouteEventHandler(ILogger<UpdateRouteEventHandler> logger, IMassTransitService<UpdateRouteEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }

    public async Task Handle(UpdateRouteEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending UpdateRouteEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("UpdateRouteEvent sent to Kafka");
    }
}
