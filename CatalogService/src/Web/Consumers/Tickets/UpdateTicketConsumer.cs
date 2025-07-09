using BuildingBlocks.Domain.Events.Tickets;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Tickets.DTO;
using CatalogService.Domain.Enum;
using CatalogService.Web.Consumers.Stations;
using MassTransit;

namespace CatalogService.Web.Consumers.Tickets;

public class UpdateTicketConsumer : IConsumer<UpdateTicketEvent>
{
    private readonly ILogger<UpdateStationConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTicketConsumer(ILogger<UpdateStationConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<UpdateTicketEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("UpdateTicketConsumer Message Received: {TicketId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var ticketReadModel = await session.LoadAsync<TicketReadModel>(message.Id);
        if (ticketReadModel != null)
        {
            ticketReadModel.Name = message.Name;
            ticketReadModel.Price = message.Price;
            ticketReadModel.ActiveInDay = message.ActiveInDay;
            ticketReadModel.ExpirationInDay = message.ExpirationInDay;
            ticketReadModel.TicketType = (TicketTypeEnum)message.TicketType;
            ticketReadModel.LastModifiedAt = message.LastModifiedAt;

            session.Update(ticketReadModel);
            await session.SaveChangesAsync();
            _logger.LogInformation("TicketReadModel Updated: {TicketId}", ticketReadModel.Id);
            return;
        }

        _logger.LogWarning("TicketReadModel not found for Id: {TicketId}", message.Id);
    }
}
