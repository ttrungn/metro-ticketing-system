using BuildingBlocks.Domain.Events.Cart;
using MassTransit;
using OrderService.Application.Carts.DTOs;
using OrderService.Application.Common.Interfaces.Repositories;

namespace OrderService.Web.Consumers.Cart;

public class AddToCartConsumer: IConsumer<AddToCartEvent>
{
    private readonly ILogger<AddToCartConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    
    public AddToCartConsumer(ILogger<AddToCartConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Consume(ConsumeContext<AddToCartEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received AddToCartEvent: {CartId}", message.Id);
        var session = _unitOfWork.GetDocumentSession();
        var cartReadModel = new CartReadModel
        {
            Id = message.Id,
            CustomerId = message.CustomerId,
            TicketId = message.TicketId,
            Quantity = message.Quantity,
            EntryStationId = message.EntryStationId,
            DestinationStationId = message.DestinationStationId,
            RouteId = message.RouteId,
            LastModifiedAt = message.LastModifiedAt,
            CreatedAt = message.CreatedAt,
            DeleteFlag = message.DeleteFlag,
            DeletedAt = message.DeletedAt
        };
        session.Store(cartReadModel);
        await session.SaveChangesAsync();
        _logger.LogInformation("CartReadModel Created: {CartId}", message.Id);
    }
}
