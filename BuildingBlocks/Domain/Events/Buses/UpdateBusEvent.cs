using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Buses;

public class UpdateBusEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
    public Guid StationId { get; set; } = Guid.Empty;
    public string DestinationName { get; set; } = null!;
}