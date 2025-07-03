using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Routes;

public class UpdateRouteEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ThumbnailImageUrl { get; set; }
    public double LengthInKm { get; set; }
}