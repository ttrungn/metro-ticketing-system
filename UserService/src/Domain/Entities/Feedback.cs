using BuildingBlocks.Domain.Common;

namespace UserService.Domain.Entities;

public class Feedback : BaseAuditableEntity<Guid>
{
    public Guid CustomerId { get; set; }
    public Guid FeedbackTypeId { get; set; }
    public Guid StationId { get; set; }
    public string Content { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public FeedbackType FeedbackType { get; set; } = null!;
}
