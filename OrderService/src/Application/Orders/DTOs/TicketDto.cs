using OrderService.Domain.Enums;

namespace OrderService.Application.Orders.DTOs;

public class TicketDto
{
    public Guid OrderId { get; set; }
    public Guid TicketId { get; set; }
    public decimal BoughtPrice { get; set; } = 0;
    public DateTimeOffset ActiveAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ExpiredAt { get; set; } = DateTimeOffset.UtcNow;
    public PurchaseTicketStatus Status { get; set; } = PurchaseTicketStatus.Unused;
    public string EntryStationId { get; set; } = null!;
    public string DestinationStationId { get; set; } = null!;
}
