using AuthService.Application.Common.Interfaces;
using BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AuthService.Infrastructure.Data.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IUser _user;
    private readonly TimeProvider _dateTime;

    public AuditableEntityInterceptor(
        IUser user,
        TimeProvider dateTime)
    {
        _user = user;
        _dateTime = dateTime;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var utcNow = _dateTime.GetUtcNow();

        foreach (var entry in context.ChangeTracker.Entries<IBaseAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.DeleteFlag = false;
                entry.Entity.DeletedAt = default;
                entry.Entity.LastModifiedAt = default;
            }
            else if (entry.State == EntityState.Modified)
            {
                // Don't modify CreatedAt on updates
                entry.Property(e => e.CreatedAt).IsModified = false;

                // Update LastModifiedAt only if properties other than audit fields changed
                if (entry.Properties.Any(p => p.IsModified && 
                                              !new[] { nameof(IBaseAuditableEntity.LastModifiedAt), 
                                                  nameof(IBaseAuditableEntity.DeletedAt), 
                                                  nameof(IBaseAuditableEntity.DeleteFlag) }.Contains(p.Metadata.Name)))
                {
                    entry.Entity.LastModifiedAt = utcNow;
                }

                // Handle soft delete
                var deleteFlagProp = entry.Property(nameof(IBaseAuditableEntity.DeleteFlag));
                if (deleteFlagProp.IsModified)
                {
                    if ((bool)deleteFlagProp.CurrentValue!)
                    {
                        entry.Entity.DeletedAt = utcNow;
                    }
                    else
                    {
                        entry.Entity.DeletedAt = default;
                    }
                }
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
