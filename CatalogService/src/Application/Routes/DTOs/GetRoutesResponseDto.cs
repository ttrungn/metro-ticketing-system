namespace CatalogService.Application.Routes.DTOs;

public class GetRoutesResponseDto
{
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public IEnumerable<RoutesResponseDto> Routes { get; set; } = new List<RoutesResponseDto>();
}
