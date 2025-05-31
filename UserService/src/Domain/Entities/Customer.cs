using BuildingBlocks.Domain.Common;

namespace UserService.Domain.Entities;

public class Customer : BaseAuditableEntity<Guid>
{
    public string ApplicationUserId { get; set; } = null!;
    public bool IsStudent { get; set; } = false;
}
