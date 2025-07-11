using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.Events.Users;

public class UpdateStudentRequestDeclinedEvent : DomainBaseEvent
{
    public Guid Id { get; set; }
    
    public Guid StaffId { get; set; }

}