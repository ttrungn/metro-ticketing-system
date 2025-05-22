namespace BuildingBlocks.Domain.Common;

public interface IBaseAuditableEntity : IBaseEntity
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset LastModifiedAt { get; set; }
    DateTimeOffset DeletedAt { get; set; }
}
