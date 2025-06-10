using BuildingBlocks.Domain.Common;

namespace CatalogService.Domain.Entities;

public class StationRoute : BaseAuditableEntity<(Guid StationId, Guid RouteId)>
{
    public Guid StationId { get; set; }
    public Station? Station { get; set; }

    public Guid RouteId { get; set; }
    public Route? Route { get; set; }

    public int Order { get; set; }
    
    public double DistanceToNext  { get; set; }
    
}
