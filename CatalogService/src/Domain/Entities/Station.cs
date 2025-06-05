using BuildingBlocks.Domain.Common;

namespace CatalogService.Domain.Entities;

public class Station : BaseAuditableEntity<Guid>
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? StreetNumber { get; set; }
    public string? Street { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? ThumbnailImageUrl { get; set; }
    
    public ICollection<StationRoute> StationRoutes { get; set; } = new List<StationRoute>();
    public ICollection<Bus> Buses { get; set; } = new List<Bus>();
}
