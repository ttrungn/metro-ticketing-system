using BuildingBlocks.Domain.Events.Cart;
using Microsoft.Extensions.Logging;
using OrderService.Application.Common.Interfaces.Services;

namespace OrderService.Application.Carts.EventHandlers;

public class DeleteCartEventHandler : INotificationHandler<DeleteCartEvent>
{
    private readonly ILogger<DeleteCartEventHandler> _logger;
    private readonly IMassTransitService<DeleteCartEvent> _massTransitService;
    
    public DeleteCartEventHandler(ILogger<DeleteCartEventHandler> logger, IMassTransitService<DeleteCartEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }
    
    public async Task Handle(DeleteCartEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending CreateStationEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("CreateStationEvent sent to Kafka");
    }
}
