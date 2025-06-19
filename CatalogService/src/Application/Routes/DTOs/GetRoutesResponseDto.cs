namespace CatalogService.Application.Routes.DTOs;

public class GetRoutesResponseDto
{
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; } = 0;
    public int PageSize { get; set; } = 8;
    public IEnumerable<RoutesResponseDto> Routes { get; set; } = new List<RoutesResponseDto>();
}
