using BuildingBlocks.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data.Configurations;

public class StudentRequestConfiguration : IEntityTypeConfiguration<StudentRequest>
{
    public void Configure(EntityTypeBuilder<StudentRequest> builder)
    {
        builder.ToTable("StudentRequest");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.StudentCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.StudentEmail)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.StudentCardImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.OwnsOne(x => x.FullName, name =>
        {
            name.Property(n => n.FirstName)
                .HasMaxLength(50)
                .IsRequired();
            name.Property(n => n.LastName)
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Staff)
            .WithMany()
            .IsRequired(false)
            .HasForeignKey(x => x.StaffId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ConfigureAuditableProperties();
    }
}
