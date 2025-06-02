using BuildingBlocks.Domain.Common;

namespace CatalogService.Domain.Entities;

public class TicketType : BaseAuditableEntity<Guid>
{
    public string? Name { get; set; }
    public int ExpirationInDays { get; set; }
    public decimal Price { get; set; }
    
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
