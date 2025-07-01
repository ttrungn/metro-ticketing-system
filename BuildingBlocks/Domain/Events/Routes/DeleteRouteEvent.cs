using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Routes;

public class DeleteRouteEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
}