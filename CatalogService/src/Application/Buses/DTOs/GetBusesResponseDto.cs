using CatalogService.Application.Stations.DTOs;

namespace CatalogService.Application.Buses.DTOs;

public class GetBusesResponseDto
{
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; } = 8;
    public IEnumerable<BusReadModel> Buses { get; set; } = new List<BusReadModel>();
}
