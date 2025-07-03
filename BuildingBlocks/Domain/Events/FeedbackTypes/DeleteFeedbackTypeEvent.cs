using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.FeedbackTypes;

public class DeleteFeedbackTypeEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
}