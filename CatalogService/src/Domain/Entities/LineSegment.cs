using BuildingBlocks.Domain.Common;

namespace CatalogService.Domain.Entities;

public class LineSegment : BaseAuditableEntity<Guid>
{
    public Guid FromStationId { get; set; }
    public Station? FromStation { get; set; }
    public Guid ToStationId { get; set; }
    public Station? ToStation { get; set; }
    public double LengthInKm { get; set; }
    public int Order { get; set; } // Order of the segment in the route
    
}
