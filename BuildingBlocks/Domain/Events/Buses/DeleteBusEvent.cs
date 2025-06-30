using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Buses;

public class DeleteBusEvent : BaseEvent
{
    public Guid Id { get; set; }
    public DateTimeOffset LastModifiedAt { get; set; }
    public DateTimeOffset DeletedAt { get; set; }
    public bool DeleteFlag { get; set; }
}