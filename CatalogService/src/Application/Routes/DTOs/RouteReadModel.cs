using BuildingBlocks.Domain.Common;

namespace CatalogService.Application.Routes.DTOs;

public class RouteReadModel : BaseReadModel
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ThumbnailImageUrl { get; set; }
    public double LengthInKm { get; set; }
}
