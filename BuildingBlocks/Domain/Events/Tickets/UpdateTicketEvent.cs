using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Tickets;

public class UpdateTicketEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
    public string? Name { get; init; }
    public decimal Price { get; init; }
    public int ActiveInDay { get; init; }
    public int ExpirationInDay { get; init; }
    public int TicketType { get; init; }
}