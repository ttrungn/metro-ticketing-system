using BuildingBlocks.Domain.Common;

namespace UserService.Domain.Entities;

public class FeedbackType : BaseAuditableEntity<Guid>
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}
