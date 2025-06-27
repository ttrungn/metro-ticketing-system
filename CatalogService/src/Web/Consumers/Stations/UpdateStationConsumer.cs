using BuildingBlocks.Domain.Events.Stations;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Stations.DTOs;
using MassTransit;

namespace CatalogService.Web.Consumers.Stations;

public class UpdateStationConsumer : IConsumer<UpdateStationEvent>
{
    private readonly ILogger<UpdateStationConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStationConsumer(ILogger<UpdateStationConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<UpdateStationEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("UpdateStationConsumer Message Received: {RouteId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var stationReadModel = await session.LoadAsync<StationReadModel>(message.Id);
        if (stationReadModel != null)
        {
            stationReadModel.Name = message.Name;
            stationReadModel.StreetNumber = message.StreetNumber;
            stationReadModel.Street = message.Street;
            stationReadModel.Ward = message.Ward;
            stationReadModel.District = message.District;
            stationReadModel.City = message.City;
            stationReadModel.ThumbnailImageUrl = message.ThumbnailImageUrl;
            stationReadModel.LastModifiedAt = message.LastModifiedAt;

            session.Update(stationReadModel);
            await session.SaveChangesAsync();
            _logger.LogInformation("StationReadModel Updated: {StationId}", stationReadModel.Id);
            return;
        }

        _logger.LogWarning("StationReadModel not found for Id: {StationId}", message.Id);
    }
}
