using BuildingBlocks.Domain.Events.Buses;
using CatalogService.Application.Buses.DTOs;
using CatalogService.Application.Common.Interfaces.Repositories;
using MassTransit;

namespace CatalogService.Web.Consumers.Buses;

public class DeleteBusConsumer : IConsumer<DeleteBusEvent>
{
    private readonly ILogger<DeleteBusConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBusConsumer(ILogger<DeleteBusConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<DeleteBusEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("DeleteRouteConsumer Message Received: {RouteId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var routeReadModel = await session.LoadAsync<BusReadModel>(message.Id);
        if (routeReadModel != null)
        {
            routeReadModel.LastModifiedAt = message.LastModifiedAt;
            routeReadModel.DeletedAt = message.DeletedAt;
            routeReadModel.DeleteFlag = message.DeleteFlag;

            session.Update(routeReadModel);
            await session.SaveChangesAsync();
            _logger.LogInformation("BusReadModel Deleted: {RouteId}", routeReadModel.Id);
            return;
        }
        _logger.LogInformation("BusReadModel not found for Id: {RouteId}", message.Id);
    }
}
