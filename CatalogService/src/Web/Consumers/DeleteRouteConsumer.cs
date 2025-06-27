using BuildingBlocks.Domain.Events.Routes;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Routes.DTOs;
using Marten.Patching;
using MassTransit;

namespace CatalogService.Web.Consumers;

public class DeleteRouteConsumer : IConsumer<DeleteRouteEvent>
{
    private readonly ILogger<DeleteRouteConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRouteConsumer(ILogger<DeleteRouteConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<DeleteRouteEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("DeleteRouteConsumer Message Received: {RouteId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var routeReadModel = await session.LoadAsync<RouteReadModel>(message.Id);
        if (routeReadModel != null)
        {
            routeReadModel.LastModifiedAt = message.LastModifiedAt;
            routeReadModel.DeletedAt = message.DeletedAt;
            routeReadModel.DeleteFlag = message.DeleteFlag;

            session.Update(routeReadModel);
            await session.SaveChangesAsync();
            _logger.LogInformation("RouteReadModel Deleted: {RouteId}", routeReadModel.Id);
            return;
        }
        _logger.LogInformation("RouteReadModel not found for Id: {RouteId}", message.Id);
    }
}
