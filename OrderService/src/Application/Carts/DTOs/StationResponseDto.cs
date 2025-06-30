namespace OrderService.Application.Carts.DTOs;

public class StationResponseDto
{
    public string Id { get; set; } = null!;
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? StreetNumber { get; set; }
    public string? Street { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? ThumbnailImageUrl { get; set; }
}
