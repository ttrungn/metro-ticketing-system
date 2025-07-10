using BuildingBlocks.Domain.Events.Tickets;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Tickets.DTO;
using MassTransit;

namespace CatalogService.Web.Consumers.Tickets;

public class DeleteTicketConsumer : IConsumer<DeleteTicketEvent>
{
    private readonly ILogger<DeleteTicketConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTicketConsumer(ILogger<DeleteTicketConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<DeleteTicketEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("DeleteTicketConsumer Message Received: {TicketId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var ticketReadModel = await session.LoadAsync<TicketReadModel>(message.Id);
        if (ticketReadModel != null)
        {
            ticketReadModel.LastModifiedAt = message.LastModifiedAt;
            ticketReadModel.DeletedAt = message.DeletedAt;
            ticketReadModel.DeleteFlag = message.DeleteFlag;

            session.Update(ticketReadModel);
            await session.SaveChangesAsync();
            _logger.LogInformation("TicketReadModel Deleted: {TicketId}", ticketReadModel.Id);
            return;
        }

        _logger.LogWarning("TicketReadModel not found for Id: {TicketId}", message.Id);
    }
}
