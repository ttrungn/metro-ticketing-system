using BuildingBlocks.Domain.Common;
using CatalogService.Domain.Enum;

namespace CatalogService.Application.Tickets.DTO;

public class TicketReadModel : BaseReadModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int ActiveInDay { get; set; }
    public int ExpirationInDay { get; set; }
    public TicketTypeEnum TicketType { get; set; }
}
