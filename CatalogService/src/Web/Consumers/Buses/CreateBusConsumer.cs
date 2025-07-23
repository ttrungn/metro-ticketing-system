using BuildingBlocks.Domain.Events.Buses;
using CatalogService.Application.Buses.DTOs;
using CatalogService.Application.Common.Interfaces.Repositories;
using MassTransit;

namespace CatalogService.Web.Consumers.Buses;

public class CreateBusConsumer : IConsumer<CreateBusEvent>
{
    private readonly ILogger<CreateBusConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBusConsumer(ILogger<CreateBusConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<CreateBusEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("CreateBusConsumer Message Received: {BusId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var busReadModel = new BusReadModel
        {
            Id = message.Id,
            Code = message.Code,
            StationId = message.StationId,
            StationName = message.StationName,
            DestinationName = message.DestinationName,
            CreatedAt = message.CreatedAt,
            LastModifiedAt = message.LastModifiedAt,
            DeletedAt = message.DeletedAt,
            DeleteFlag = message.DeleteFlag,
        };

        session.Store(busReadModel);
        await session.SaveChangesAsync();
        _logger.LogInformation("BusReadModel Created: {BusId}", busReadModel.Id);
    }
}
