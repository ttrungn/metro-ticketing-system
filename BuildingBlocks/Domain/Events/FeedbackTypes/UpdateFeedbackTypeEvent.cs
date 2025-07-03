using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.FeedbackTypes;

public class UpdateFeedbackTypeEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}