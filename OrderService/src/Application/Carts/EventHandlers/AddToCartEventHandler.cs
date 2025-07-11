using BuildingBlocks.Domain.Events.Cart;
using Microsoft.Extensions.Logging;
using OrderService.Application.Common.Interfaces.Services;

namespace OrderService.Application.Carts.EventHandlers;

public class AddToCartEventHandler : INotificationHandler<AddToCartEvent>
{
    private readonly ILogger<AddToCartEventHandler> _logger;
    private readonly IMassTransitService<AddToCartEvent> _massTransitService;

    public AddToCartEventHandler(ILogger<AddToCartEventHandler> logger, IMassTransitService<AddToCartEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }

    public async Task Handle(AddToCartEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending AddToCartEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("AddToCartEvent sent to Kafka");
    }
    
}
