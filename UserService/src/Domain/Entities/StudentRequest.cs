using BuildingBlocks.Domain.Common;
using UserService.Domain.Enums;
using UserService.Domain.ValueObjects;

namespace UserService.Domain.Entities;

public class StudentRequest : BaseAuditableEntity<Guid>
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public Guid StaffId { get; set; }
    public Staff? Staff { get; set; }
    public string StudentCode { get; set; } = null!;
    public string StudentEmail { get; set; } = null!;
    public FullName FullName { get; set; } = null!;
    public DateTimeOffset DateOfBirth { get; set; }
    public string StudentCardImageUrl { get; set; } = null!;
    public StudentRequestStatus Status { get; set; } = StudentRequestStatus.Pending;
}
