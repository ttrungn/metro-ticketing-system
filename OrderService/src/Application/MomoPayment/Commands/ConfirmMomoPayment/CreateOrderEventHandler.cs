using BuildingBlocks.Domain.Events.Buses;
using BuildingBlocks.Domain.Events.Orders;
using Microsoft.Extensions.Logging;
using OrderService.Application.Common.Interfaces.Services;

namespace OrderService.Application.MomoPayment.Commands.ConfirmMomoPayment;

public class CreateOrderEventHandler : INotificationHandler<CreateOrderEvent>
{
    private readonly ILogger<CreateOrderEventHandler> _logger;
    private readonly IMassTransitService<CreateOrderEvent> _massTransitService;

    public CreateOrderEventHandler(IMassTransitService<CreateOrderEvent> massTransitService, ILogger<CreateOrderEventHandler> logger)
    {
        _massTransitService = massTransitService;
        _logger = logger;
    }

    public async Task Handle(CreateOrderEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending CreateOrderEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("CreateOrderEvent sent to Kafka");
    }
}
