using BuildingBlocks.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Infrastructure.Services.Identity;

namespace UserService.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.OwnsOne(u => u.FullName, fn =>
        {
            fn.Property(f => f.FirstName).HasColumnName("FirstName").IsRequired().HasMaxLength(100);
            fn.Property(f => f.LastName).HasColumnName("LastName").IsRequired().HasMaxLength(100);
        });

        builder.ConfigureAuditableProperties();

        builder.Ignore(u => u.DomainEvents);
    }
}
