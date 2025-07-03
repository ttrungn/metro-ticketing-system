using BuildingBlocks.Domain.Events.Stations;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Stations.DTOs;
using MassTransit;

namespace CatalogService.Web.Consumers.Stations;

public class CreateStationConsumer : IConsumer<CreateStationEvent>
{
    private readonly ILogger<CreateStationConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStationConsumer(ILogger<CreateStationConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<CreateStationEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("CreateStationConsumer Message Received: {StationId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var stationReadModel = new StationReadModel()
        {
            Id = message.Id,
            Code = message.Code,
            Name = message.Name,
            StreetNumber = message.StreetNumber,
            Street = message.Street,
            Ward = message.Ward,
            District = message.District,
            City = message.City,
            ThumbnailImageUrl = message.ThumbnailImageUrl,
            CreatedAt = message.CreatedAt,
            LastModifiedAt = message.LastModifiedAt,
            DeletedAt = message.DeletedAt,
            DeleteFlag = message.DeleteFlag
        };

        session.Store(stationReadModel);
        await session.SaveChangesAsync();
        _logger.LogInformation("StationReadModel Created: {StationId}", stationReadModel.Id);
    }
}
