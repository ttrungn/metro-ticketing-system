using BuildingBlocks.Domain.Events.Buses;
using CatalogService.Application.Buses.DTOs;
using CatalogService.Application.Common.Interfaces.Repositories;
using MassTransit;

namespace CatalogService.Web.Consumers.Buses;

public class UpdateBusConsumer : IConsumer<UpdateBusEvent>
{
    private readonly ILogger<UpdateBusConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBusConsumer(ILogger<UpdateBusConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<UpdateBusEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("UpdateBusConsumer Message Received: {BusId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var busReadModel = await session.LoadAsync<BusReadModel>(message.Id);
        if (busReadModel != null)
        {
            busReadModel.StationId = message.StationId;
            busReadModel.DestinationName = message.DestinationName;
            busReadModel.LastModifiedAt = message.LastModifiedAt;

            session.Update(busReadModel);
            await session.SaveChangesAsync();
            _logger.LogInformation("BusReadModel Updated: {BusId}", busReadModel.Id);
            return;
        }

        _logger.LogWarning("BusReadModel not found for Id: {BusId}", message.Id);
    }
}
