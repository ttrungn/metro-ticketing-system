using BuildingBlocks.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data.Configurations;

public class FeedbackTypeConfiguration : IEntityTypeConfiguration<FeedbackType>
{
    public void Configure(EntityTypeBuilder<FeedbackType> builder)
    {
        builder.ToTable("FeedbackType");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.ConfigureAuditableProperties();
    }
}
