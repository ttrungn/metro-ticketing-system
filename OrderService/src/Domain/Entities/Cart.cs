using BuildingBlocks.Domain.Common;

namespace OrderService.Domain.Entities;

public class Cart : BaseAuditableEntity<Guid>
{
    public string CustomerId { get; set; } = null!;
    public string TicketId { get; set; } = null!;
    public int Quantity { get; set; }
    public string? EntryStationId { get; set; }
    public string? DestinationStationId { get; set; }
    public string? RouteId { get; set; }
}
