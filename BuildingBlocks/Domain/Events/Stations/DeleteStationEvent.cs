using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Stations;

public class DeleteStationEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
}