using BuildingBlocks.Domain.Common;

namespace CatalogService.Domain.Entities;

public class Ticket : BaseAuditableEntity<Guid>
{
    public string? Name { get; set; }
    public int ExpirationInDay { get; set; }
    public decimal Price { get; set; }
    public bool IsOneWay { get; set; }
    public Guid TicketTypeId { get; set; }
    
}
