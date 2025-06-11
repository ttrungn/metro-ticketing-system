using BuildingBlocks.Domain.Common;

namespace CatalogService.Domain.Entities;

public class Bus : BaseAuditableEntity<Guid>
{
    public string? Code { get; set; }
    public Guid StationId { get; set; }
    public Station? Station { get; set; }
    public string? DestinationName { get; set; }
}
