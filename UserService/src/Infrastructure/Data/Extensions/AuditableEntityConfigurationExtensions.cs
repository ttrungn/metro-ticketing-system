using BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserService.Infrastructure.Data.Extensions;

public static class AuditableEntityConfigurationExtensions
{
    public static void ConfigureAuditableProperties<T>(this EntityTypeBuilder<T> builder) where T : class
    {
        builder.Property(nameof(IBaseAuditableEntity.CreatedAt))
            .IsRequired();

        builder.Property(nameof(IBaseAuditableEntity.LastModifiedAt))
            .IsRequired();

        builder.Property(nameof(IBaseAuditableEntity.DeletedAt))
            .IsRequired(false);

        builder.Property(nameof(IBaseAuditableEntity.DeleteFlag))
            .IsRequired();
    }
}
