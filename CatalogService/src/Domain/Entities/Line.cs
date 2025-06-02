using BuildingBlocks.Domain.Common;

namespace CatalogService.Domain.Entities;

public class Line : BaseAuditableEntity<Guid>
{
    public Guid RouteId { get; set; }
    public Route? Route { get; set; }
    public Guid EntryStationId { get; set; }
    public Station? EntryStation { get; set; }
    public Guid ExitStationId { get; set; }
    public Station? ExitStation { get; set; }
    public double TotalLengthInKm { get; set; }
    public string? Code { get; set; }   
    public decimal Price { get; set; }
    
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    
}
