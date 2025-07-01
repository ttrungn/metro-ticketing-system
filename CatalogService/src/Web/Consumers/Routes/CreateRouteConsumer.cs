using BuildingBlocks.Domain.Events.Routes;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Routes.DTOs;
using MassTransit;

namespace CatalogService.Web.Consumers.Routes;

public class CreateRouteConsumer : IConsumer<CreateRouteEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateRouteConsumer> _logger;

    public CreateRouteConsumer(ILogger<CreateRouteConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<CreateRouteEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("CreateRouteConsumer Message Received: {RouteId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var routeReadModel = new RouteReadModel()
        {
            Id = message.Id,
            Code = message.Code,
            Name = message.Name,
            ThumbnailImageUrl = message.ThumbnailImageUrl,
            LengthInKm = message.LengthInKm,
            CreatedAt = message.CreatedAt,
            LastModifiedAt = message.LastModifiedAt,
            DeletedAt = message.DeletedAt,
            DeleteFlag = message.DeleteFlag,
        };

        session.Store(routeReadModel);
        await session.SaveChangesAsync();
        _logger.LogInformation("RouteReadModel Created: {RouteId}", routeReadModel.Id);
    }
}
