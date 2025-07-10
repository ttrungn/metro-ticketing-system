using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Tickets;

public class DeleteTicketEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
}