using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Routes;

public class CreateRouteEvent : BaseEvent
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ThumbnailImageUrl { get; set; }
    public double LengthInKm { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}