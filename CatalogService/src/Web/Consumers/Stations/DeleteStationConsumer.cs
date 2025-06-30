using BuildingBlocks.Domain.Events.Stations;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Stations.DTOs;
using MassTransit;

namespace CatalogService.Web.Consumers.Stations;

public class DeleteStationConsumer : IConsumer<DeleteStationEvent>
{
    private readonly ILogger<DeleteStationConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteStationConsumer(ILogger<DeleteStationConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<DeleteStationEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("DeleteStationConsumer Message Received: {RouteId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var stationReadModel = await session.LoadAsync<StationReadModel>(message.Id);
        if (stationReadModel != null)
        {
            stationReadModel.LastModifiedAt = message.LastModifiedAt;
            stationReadModel.DeletedAt = message.DeletedAt;
            stationReadModel.DeleteFlag = message.DeleteFlag;

            session.Update(stationReadModel);
            await session.SaveChangesAsync();
            _logger.LogInformation("StationReadModel Deleted: {StationId}", stationReadModel.Id);
            return;
        }
        _logger.LogInformation("StationReadModel not found for Id: {StationId}", message.Id);
    }
}
