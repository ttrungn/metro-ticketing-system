using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Buses;

public class CreateBusEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public Guid StationId { get; set; } = Guid.Empty;
    public string? StationName { get; set; } = string.Empty;
    public string DestinationName { get; set; } = null!;
}