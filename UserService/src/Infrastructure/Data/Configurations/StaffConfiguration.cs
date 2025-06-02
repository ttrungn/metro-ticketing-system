using BuildingBlocks.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data.Configurations
{
    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            // Map to table "Staff" (or "Staffs" if you prefer plural)
            builder.ToTable("Staff");

            // Primary key (inherited as Id from BaseAuditableEntity<Guid>)
            builder.HasKey(s => s.Id);

            // Configure ApplicationUserId as required, max length 450 (to match ASP.NET Identity default)
            builder
                .Property(s => s.ApplicationUserId)
                .IsRequired()
                .HasMaxLength(450);

            // (Optional) Index on ApplicationUserId for faster lookups:
            builder
                .HasIndex(s => s.ApplicationUserId)
                .HasDatabaseName("IX_Staff_ApplicationUserId")
                .IsUnique(false);

            builder.ConfigureAuditableProperties();

            builder.Ignore(u => u.DomainEvents);
        }
    }
}
