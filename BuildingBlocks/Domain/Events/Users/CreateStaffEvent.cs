using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Users;

public class CreateStaffEvent : DomainBaseEvent
{
    public string ApplicationUserId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Guid StaffId { get; set; }
    public string Email { get; set; } = null!;
    public bool DeleteFlag { get; set; }
}