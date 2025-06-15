using BuildingBlocks.Domain.Common;
using CatalogService.Domain.Enum;

namespace CatalogService.Domain.Entities;

public class Ticket : BaseAuditableEntity<Guid>
{
    public string? Name { get; set; }
    public int ExpirationInDay { get; set; }
    public decimal Price { get; set; }
    public TicketTypeEnum TicketType { get; set; }

    public int ActiveInDay { get; set; } 

}
