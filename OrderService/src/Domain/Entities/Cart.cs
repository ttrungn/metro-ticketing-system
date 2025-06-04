using BuildingBlocks.Domain.Common;

namespace OrderService.Domain.Entities;

public class Cart
{
    public string CustomerId { get; set; } = null!;
    public string TicketId { get; set; } = null!;
    public int Quantity { get; set; }
}
