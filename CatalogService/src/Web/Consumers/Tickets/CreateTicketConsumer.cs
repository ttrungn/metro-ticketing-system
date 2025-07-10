using BuildingBlocks.Domain.Events.Tickets;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Tickets.DTO;
using CatalogService.Domain.Enum;
using MassTransit;

namespace CatalogService.Web.Consumers.Tickets;

public class CreateTicketConsumer : IConsumer<CreateTicketEvent>
{
    private readonly ILogger<CreateTicketConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTicketConsumer(ILogger<CreateTicketConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<CreateTicketEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("CreateTicketConsumer Message Received: {Id}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var ticketReadModel = new TicketReadModel()
        {
            Id = message.Id,
            Name = message.Name,
            Price = message.Price,
            ActiveInDay = message.ActiveInDay,
            ExpirationInDay = message.ExpirationInDay,
            TicketType = (TicketTypeEnum)message.TicketType,
            CreatedAt = message.CreatedAt,
            LastModifiedAt = message.LastModifiedAt,
            DeletedAt = message.DeletedAt,
            DeleteFlag = message.DeleteFlag
        };

        session.Store(ticketReadModel);
        await session.SaveChangesAsync();

        _logger.LogInformation("TicketReadModel Created: {TicketId}", ticketReadModel.Id);
    }
}
