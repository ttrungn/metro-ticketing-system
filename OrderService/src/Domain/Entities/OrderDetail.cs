using BuildingBlocks.Domain.Common;

namespace OrderService.Domain.Entities;

public class OrderDetail : BaseAuditableEntity<(Guid OrderId, Guid PurchaseTicketId)>
{
    public Guid OrderId { get; set; }
    public Guid PurchaseTicketId { get; set; }
    public decimal BoughtPrice { get; set; } = 0;
    public Order Order { get; set; } = null!;
    public PurchasedTicket PurchasedTicket { get; set; } = null!;
}
