using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Common;
using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities;
public class TransactionHistory : BaseAuditableEntity<Guid>
{
    public Guid OrderId { get; set; } = Guid.Empty;
    public string CustomerId { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.MoMo!;

    public TransactionStatus Status;

    public string TransactionType { get; set; } = null!;
}
