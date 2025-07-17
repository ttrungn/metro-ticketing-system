using BuildingBlocks.Domain.Common;

namespace OrderService.Application.Orders.DTOs;

public class TicketReadModel : BaseReadModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int ActiveInDay { get; set; }
    public int ExpirationInDay { get; set; }
    public int TicketType { get; set; }
}
