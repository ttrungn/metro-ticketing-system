using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Users;

public class CreateStudentRequestEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? StaffId { get; set; }
    public string StudentCode { get; set; } = null!;
    public string StudentEmail { get; set; } = null!;
    
    public string SchoolName { get; set; } = null!;
    
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTimeOffset DateOfBirth { get; set; }
    public string StudentCardImageUrl { get; set; } = null!;
}