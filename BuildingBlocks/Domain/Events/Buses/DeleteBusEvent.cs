using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Buses;

public class DeleteBusEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
}