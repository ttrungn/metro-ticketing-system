namespace OrderService.Application.Carts.DTOs;

public class GetRouteResponseDto
{
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; } = 0;
    public int PageSize { get; set; } = 8;
    public IEnumerable<RouteResponseDto> Routes { get; set; } = new List<RouteResponseDto>();
}
