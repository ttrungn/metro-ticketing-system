using BuildingBlocks.Domain.Events.Orders;
using MassTransit;
using NotificationService.Application.Mails.Queries.SendCreateOrder;

namespace NotificationService.Web.Consumers;

public class CreateOrderEventConsumer : IConsumer<CreateOrderEvent>
{
    private readonly ILogger<CreateOrderEventConsumer> _logger;
    private readonly ISender _sender;
    
    public CreateOrderEventConsumer(ILogger<CreateOrderEventConsumer> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }
    
    public async Task Consume(ConsumeContext<CreateOrderEvent> context)
    {
        var query = new SendCreateOrderQuery()
        {
            Email = context.Message.Email,
            OrderId = context.Message.OrderId,
            Amount = context.Message.Amount,
            OrderDetails = context.Message.OrderDetails.Select(od => new OrderDetailRequestDto()
            {
                TicketId = od.TicketId,
                EntryStationId = od.EntryStationId ?? Guid.Empty,
                DestinationStationId = od.DestinationStationId ?? Guid.Empty,
                Quantity = od.Quantity,
                Price = od.Price
            }).ToList(),
        };
        
        await _sender.Send(query);
    }
}
