using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Users;

public class UpdateStudentRequestApproveEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
    
    public Guid StaffId { get; set; }
    
    
}