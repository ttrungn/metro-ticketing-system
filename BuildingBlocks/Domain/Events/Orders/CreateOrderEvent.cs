using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Orders;

public class CreateOrderEvent : DomainBaseEvent
{
    public string Email { get; init; } = null!;
    public Guid OrderId { get; init; }
    public decimal Amount { get; init; }
    public List<CreateOrderEventOrderDetail> OrderDetails { get; init; } = null!;
}