using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Cart;

public class AddToCartEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = null!;
    public string TicketId { get; set; } = null!;
    public int Quantity { get; set; }
    public string EntryStationId { get; set; } = null!;
    public string DestinationStationId { get; set; } = null!;
    public string RouteId { get; set; } = null!;
}