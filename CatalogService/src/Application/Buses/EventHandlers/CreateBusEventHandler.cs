using BuildingBlocks.Domain.Events.Buses;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Buses.EventHandlers;

public class CreateBusEventHandler : INotificationHandler<CreateBusEvent>
{
    private readonly ILogger<CreateBusEventHandler> _logger;
    private readonly IMassTransitService<CreateBusEvent> _massTransitService;

    public CreateBusEventHandler(IMassTransitService<CreateBusEvent> massTransitService, ILogger<CreateBusEventHandler> logger)
    {
        _massTransitService = massTransitService;
        _logger = logger;
    }

    public async Task Handle(CreateBusEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending CreateBusEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("CreateBusEvent sent to Kafka");
    }
}
