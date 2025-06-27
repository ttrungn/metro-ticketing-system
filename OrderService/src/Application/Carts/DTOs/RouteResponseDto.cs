namespace OrderService.Application.Carts.DTOs;

public class RouteResponseDto
{
    public string Id { get; set; } = null!;
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? ThumbnailImageUrl { get; set; }
    public double LengthInKm { get; set; }
}
