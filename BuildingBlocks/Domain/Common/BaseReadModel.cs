namespace BuildingBlocks.Domain.Common;

public abstract class BaseReadModel
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastModifiedAt { get; set; } = DateTimeOffset.MinValue;
    public DateTimeOffset? DeletedAt { get; set; } = DateTimeOffset.MinValue;
    public bool DeleteFlag { get; set; } = false;
}