using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Stations;

public class UpdateStationEvent : BaseEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? StreetNumber { get; set; }
    public string? Street { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? ThumbnailImageUrl { get; set; }
    public DateTimeOffset LastModifiedAt { get; set; }
}