using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Buses;

public class UpdateBusEvent : BaseEvent
{
    public Guid Id { get; set; }
    public Guid StationId { get; set; } = Guid.Empty;
    public string DestinationName { get; set; } = null!;
    public DateTimeOffset LastModifiedAt { get; set; }
}