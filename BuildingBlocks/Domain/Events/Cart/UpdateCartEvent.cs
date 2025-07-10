using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Cart;

public class UpdateCartEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
}