namespace BuildingBlocks.Domain.Common;

public class DomainBaseEvent : BaseEvent
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public bool DeleteFlag { get; set; } = false;
}