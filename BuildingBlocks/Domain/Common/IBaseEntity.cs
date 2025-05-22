namespace BuildingBlocks.Domain.Common;

public interface IBaseEntity
{
    bool DeleteFlag { get; set; }
    IReadOnlyCollection<BaseEvent> DomainEvents { get; }
    void AddDomainEvent(BaseEvent domainEvent);
    void RemoveDomainEvent(BaseEvent domainEvent);
    void ClearDomainEvents();
}
