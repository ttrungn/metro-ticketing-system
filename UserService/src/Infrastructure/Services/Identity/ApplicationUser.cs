using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.Domain.Common;
using Microsoft.AspNetCore.Identity;
using UserService.Domain.ValueObjects;

namespace UserService.Infrastructure.Services.Identity;

public class ApplicationUser : IdentityUser, IBaseAuditableEntity, IBaseEntity
{
    public FullName FullName { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastModifiedAt { get; set; }
    public DateTimeOffset DeletedAt { get; set; }
    public bool DeleteFlag { get; set; }
    private readonly List<BaseEvent> _domainEvents = [];
    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public ApplicationUser() : base()
    {
        CreatedAt = DateTimeOffset.UtcNow;
        LastModifiedAt = DateTimeOffset.UtcNow;
        DeleteFlag = false;
    }

    public ApplicationUser(string username, string email) : this()
    {
        UserName = username;
        Email = email;
    }

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
