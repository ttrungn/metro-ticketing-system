using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Feedbacks;

public class CreateFeedbackEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid FeedbackTypeId { get; set; }
    public Guid StationId { get; set; }
    public string Content { get; set; } = null!;
}