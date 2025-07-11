using BuildingBlocks.Domain.Events.Cart;
using Microsoft.Extensions.Logging;
using OrderService.Application.Common.Interfaces.Services;

namespace OrderService.Application.Carts.EventHandlers;

public class UpdateCartEventHandler : INotificationHandler<UpdateCartEvent>
{
    private readonly ILogger<UpdateCartEventHandler> _logger;
    private readonly IMassTransitService<UpdateCartEvent> _massTransitService;

    public UpdateCartEventHandler(ILogger<UpdateCartEventHandler> logger, IMassTransitService<UpdateCartEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }

    public async Task Handle(UpdateCartEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending UpdateCartEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("UpdateCartEvent sent to Kafka");
    }
}
