using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Cart;

public class DeleteCartEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
}