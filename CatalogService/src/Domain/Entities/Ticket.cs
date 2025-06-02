using BuildingBlocks.Domain.Common;

namespace CatalogService.Domain.Entities;

public class Ticket : BaseAuditableEntity<Guid>
{
    public string? Name { get; set; }
    
    public Guid TicketTypeId { get; set; }
    public TicketType? TicketType { get; set; }
    public Guid LineId { get; set; }
    public Line? Line { get; set; }
    
}
