using BuildingBlocks.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            // Map to table "Customers"
            builder.ToTable("Customers");

            // Primary key (inherited as Id from BaseAuditableEntity<Guid>)
            builder.HasKey(c => c.Id);

            // Configure ApplicationUserId as required, max length 450 (to match ASP.NET Identity default)
            builder
                .Property(c => c.ApplicationUserId)
                .IsRequired()
                .HasMaxLength(450);

            // Configure IsStudent as required (non-nullable)
            builder
                .Property(c => c.IsStudent)
                .IsRequired();

            // (Optional) If you want to look up Customer by ApplicationUserId often, add an index:
            builder
                .HasIndex(c => c.ApplicationUserId)
                .HasDatabaseName("IX_Customers_ApplicationUserId")
                .IsUnique(false);

            builder.ConfigureAuditableProperties();

            builder.Ignore(u => u.DomainEvents);
        }
    }
}
