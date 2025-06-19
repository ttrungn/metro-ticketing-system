namespace CatalogService.Application.Routes.DTOs;

public class StationRouteResponseDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public double LengthInKm { get; set; }
    public string? ThumbnailImageUrl { get; set; }
    public IEnumerable<StationResponseDto> Stations { get; set; } = new List<StationResponseDto>();
}
