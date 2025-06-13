namespace CatalogService.Application.Routes.DTOs;

public class RouteWithStationResponseDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? ThumbnailImageUrl { get; set; }
    public double LengthInKm { get; set; }
}
