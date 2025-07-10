using BuildingBlocks.Domain.Events.Cart;
using MassTransit;
using OrderService.Application.Carts.DTOs;
using OrderService.Application.Common.Interfaces.Repositories;

namespace OrderService.Web.Consumers.Cart;

public class UpdateCartConsumer : IConsumer<UpdateCartEvent>
{
    private readonly ILogger<UpdateCartConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    
    public UpdateCartConsumer(ILogger<UpdateCartConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Consume(ConsumeContext<UpdateCartEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("UpdateCartConsumer Message Received for Cart ID: {CartId}", message.Id);
        var session = _unitOfWork.GetDocumentSession();
        var cart = await session.LoadAsync<CartReadModel>(message.Id);
        if(cart != null)
        {
            cart.Quantity = message.Quantity;
            session.Store(cart);
            await session.SaveChangesAsync();
            _logger.LogInformation("CartID updated: {CartId}", message.Id);
            return;
        }
        _logger.LogWarning("CartID not found for Id: {CartId}", message.Id);
    }
}
