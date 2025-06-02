using BuildingBlocks.Domain.Common;
using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities;

public class PurchasedTicket : BaseAuditableEntity<Guid>
{
    public string TicketId { get; set; } = null!;
    public string CustomerId { get; set; } = null!;
    public DateTimeOffset ExpiredAt { get; set; }
    public PurchaseTicketStatus Status { get; set; } = PurchaseTicketStatus.Unused;
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Unpaid;

    public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

}
