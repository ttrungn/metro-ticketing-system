using BuildingBlocks.Domain.Common;
using MediatR;

namespace BuildingBlocks.Domain.Events.Users;

public class CreateCustomerEvent : DomainBaseEvent
{
    public string ApplicationUserId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public string Email { get; set; } = null!;
    public bool IsStudent { get; set; } = false;
    public bool DeleteFlag { get; set; }
}