using BuildingBlocks.Domain.Common;

namespace UserService.Application.Feedbacks.DTOs;

public class StationReadModel : BaseReadModel
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? StreetNumber { get; set; }
    public string? Street { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? ThumbnailImageUrl { get; set; }
}
