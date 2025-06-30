using BuildingBlocks.Domain.Events.Routes;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Routes.EventHandlers;

public class DeleteRouteEventHandler : INotificationHandler<DeleteRouteEvent>
{
    private readonly ILogger<DeleteRouteEventHandler> _logger;
    private readonly IMassTransitService<DeleteRouteEvent> _massTransitService;

    public DeleteRouteEventHandler(ILogger<DeleteRouteEventHandler> logger, IMassTransitService<DeleteRouteEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }

    public async Task Handle(DeleteRouteEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending DeleteRouteEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("DeleteRouteEvent sent to Kafka");
    }
}
