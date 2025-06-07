namespace CatalogService.Application.Stations.DTOs;

public class GetStationsResponseDto
{
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public IEnumerable<StationsResponseDto> Stations { get; set; } = new List<StationsResponseDto>();
}
