using BuildingBlocks.Domain.Common;

namespace UserService.Domain.Entities;

public class Feedback : BaseAuditableEntity<Guid>
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public Guid FeedbackTypeId { get; set; }
    public FeedbackType FeedbackType { get; set; } = null!;
    public string Comment { get; set; } = null!;
    public string Location { get; set; } = null!;
}
