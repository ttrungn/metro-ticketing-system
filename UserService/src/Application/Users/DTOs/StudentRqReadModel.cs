using BuildingBlocks.Domain.Common;
using UserService.Domain.Enums;
using UserService.Domain.ValueObjects;

namespace UserService.Application.Users.DTOs;

public class StudentRqReadModel : BaseReadModel
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? StaffId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string SchoolName { get; set; } = string.Empty;
    public FullName FullName { get; set; } = null!; 
    public DateTimeOffset DateOfBirth { get; set; }
    public string StudentCardImageUrl { get; set; } = string.Empty;
    public StudentRequestStatus Status { get; set; } 
}
