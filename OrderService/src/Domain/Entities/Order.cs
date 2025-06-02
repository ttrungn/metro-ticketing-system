using BuildingBlocks.Domain.Common;
using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities;

public class Order : BaseAuditableEntity<Guid>
{
    public string CustomerId { get; set; } = null!;
    public string ThirdPartyPaymentId { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.VNPay;

}
