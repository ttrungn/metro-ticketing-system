﻿namespace BuildingBlocks.Domain.Common;

public abstract class BaseAuditableEntity<TId> : BaseEntity<TId>, IBaseAuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastModifiedAt { get; set; }
    public DateTimeOffset DeletedAt { get; set; }
}
