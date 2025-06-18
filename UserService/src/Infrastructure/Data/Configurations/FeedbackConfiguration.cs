using BuildingBlocks.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data.Configurations
{
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.ToTable("Feedback");

            builder.Property(f => f.Content)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(f => f.StationId)
                .IsRequired();

            builder.HasOne(f => f.Customer)
                .WithMany()
                .HasForeignKey(f => f.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(f => f.FeedbackType)
                .WithMany()
                .HasForeignKey(f => f.FeedbackTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.ConfigureAuditableProperties();
        }
    }
}
