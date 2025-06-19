namespace UserService.Application.Feedbacks.DTOs;

public class StationsResponseDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? StreetNumber { get; set; }
    public string? Street { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? ThumbnailImageUrl { get; set; }
}
