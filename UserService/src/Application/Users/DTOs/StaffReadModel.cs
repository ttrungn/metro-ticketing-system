using BuildingBlocks.Domain.Common;
using UserService.Domain.ValueObjects;

namespace UserService.Application.Users.DTOs;

public class StaffReadModel : BaseReadModel
{
    public string Id { get; set; } = null!;
    public FullName FullName { get; set; } = null!;
    public string? Email { get; set; } = null!;
    public Guid StaffId { get; set; }
}
