namespace CatalogService.Domain.Entities;

public class StationRoute
{
    public Guid StationId { get; set; }
    public Station? Station { get; set; }

    public Guid RouteId { get; set; }
    public Route? Route { get; set; }
}
