using BuildingBlocks.Domain.Common;

namespace OrderService.Domain.Entities;

public class OrderDetail : BaseAuditableEntity<OrderDetail>
{
    public string OrderId { get; set; } = null!;
    public string PurchaseTicketId { get; set; } = null!;
    public decimal BoughtPrice { get; set; } = 0;
}
