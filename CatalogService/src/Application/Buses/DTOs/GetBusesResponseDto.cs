using CatalogService.Application.Stations.DTOs;

namespace CatalogService.Application.Buses.DTOs;

public class GetBusesResponseDto
{
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public IEnumerable<BusResponseDto> Buses { get; set; } = new List<BusResponseDto>();
}
