using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Data.Configurations;
public class TransactionHistoryConfiguration : IEntityTypeConfiguration<TransactionHistory>
{
    public void Configure(EntityTypeBuilder<TransactionHistory> builder)
    {
        builder.HasKey(t => t.Id);

        builder.ToTable("TransactionHistory");

        builder.Property(t => t.OrderId).IsRequired();

        builder.Property(t => t.Amount).IsRequired().HasPrecision(18, 2);

        builder.Property(t => t.CustomerId).IsRequired().HasMaxLength(200);

        builder.Property(t => t.Status).IsRequired();

        builder.Property(t => t.TransactionType).IsRequired().HasMaxLength(500);

        builder.Property(t => t.TransactionDate).IsRequired();

        builder.Property(t => t.PaymentMethod).IsRequired();
    }
}
