using BuildingBlocks.Domain.Events.Routes;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Routes.DTOs;
using MassTransit;

namespace CatalogService.Web.Consumers.Routes;

public class UpdateRouteConsumer : IConsumer<UpdateRouteEvent>
{
    private readonly ILogger<UpdateRouteConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRouteConsumer(ILogger<UpdateRouteConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<UpdateRouteEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("UpdateRouteConsumer Message Received: {RouteId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var routeReadModel = await session.LoadAsync<RouteReadModel>(message.Id);
        if (routeReadModel != null)
        {
            routeReadModel.Name = message.Name;
            routeReadModel.ThumbnailImageUrl = message.ThumbnailImageUrl;
            routeReadModel.LengthInKm = message.LengthInKm;
            routeReadModel.LastModifiedAt = message.LastModifiedAt;

            session.Update(routeReadModel);
            await session.SaveChangesAsync();
            _logger.LogInformation("RouteReadModel Updated: {RouteId}", routeReadModel.Id);
            return;
        }

        _logger.LogWarning("RouteReadModel not found for Id: {RouteId}", message.Id);
    }
}
