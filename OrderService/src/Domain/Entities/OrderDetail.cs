using BuildingBlocks.Domain.Common;
using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities;

public class OrderDetail : BaseAuditableEntity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid TicketId { get; set; }
    public decimal BoughtPrice { get; set; } = 0;
    public DateTimeOffset ActiveAt { get; set; } = DateTimeOffset.MinValue;
    public DateTimeOffset ExpiredAt { get; set; } = DateTimeOffset.UtcNow;
    public PurchaseTicketStatus Status { get; set; } = PurchaseTicketStatus.Unused;
    public string? EntryStationId { get; set; } = null!;
    public string? DestinationStationId { get; set; } = null!;
    public Order Order { get; set; } = null!;
}
