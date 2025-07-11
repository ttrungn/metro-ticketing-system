using BuildingBlocks.Domain.Events.Cart;
using MassTransit;
using OrderService.Application.Carts.DTOs;
using OrderService.Application.Common.Interfaces.Repositories;

namespace OrderService.Web.Consumers.Cart;

public class DeleteCartConsumer : IConsumer<DeleteCartEvent>
{
    private readonly ILogger<DeleteCartConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCartConsumer(ILogger<DeleteCartConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Consume(ConsumeContext<DeleteCartEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("DeleteCartConsumer Message Received: {RouteId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var routeReadModel = await session.LoadAsync<CartReadModel>(message.Id);
        if (routeReadModel != null)
        { 
            session.Delete<CartReadModel>(message.Id);
            await session.SaveChangesAsync();
            _logger.LogInformation("CartReadModel Deleted: {RouteId}", routeReadModel.Id);
            return;
        }
        _logger.LogInformation("CartReadModel not found for Id: {RouteId}", message.Id);
    }
}
