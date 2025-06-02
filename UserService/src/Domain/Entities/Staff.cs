using BuildingBlocks.Domain.Common;

namespace UserService.Domain.Entities;

public class Staff : BaseAuditableEntity<Guid>
{
    public string ApplicationUserId { get; set; } = null!;
}
