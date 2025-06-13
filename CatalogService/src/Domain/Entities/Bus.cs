using BuildingBlocks.Domain.Common;

namespace CatalogService.Domain.Entities;

public class Bus : BaseAuditableEntity<Guid>
{
    public string Code { get; set; } = null!;
    public Guid StationId { get; set; } = Guid.Empty;
    public Station? Station { get; set; }
    public string DestinationName { get; set; } = null!;
    
}
