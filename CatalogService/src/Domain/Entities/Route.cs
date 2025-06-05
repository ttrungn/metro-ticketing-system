using BuildingBlocks.Domain.Common;

namespace CatalogService.Domain.Entities;

public class Route : BaseAuditableEntity<Guid>
{
     public string? Code { get; set; }
     public string? Name { get; set; }
     public string? ThumbnailImageUrl { get; set; }
     public double LengthInKm { get; set; }
     
     public ICollection<StationRoute> StationRoutes { get; set; } = new List<StationRoute>();
}
