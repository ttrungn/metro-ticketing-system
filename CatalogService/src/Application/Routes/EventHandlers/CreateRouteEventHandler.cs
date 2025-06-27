using BuildingBlocks.Domain.Events.Routes;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Routes.EventHandlers;

public class CreateRouteEventHandler : INotificationHandler<CreateRouteEvent>
{
    private readonly ILogger<CreateRouteEventHandler> _logger;
    private readonly IMassTransitService<CreateRouteEvent> _massTransitService;

    public CreateRouteEventHandler(ILogger<CreateRouteEventHandler> logger, IMassTransitService<CreateRouteEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }

    public async Task Handle(CreateRouteEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending CreateRouteEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("CreateRouteEvent sent to Kafka");
    }
}
