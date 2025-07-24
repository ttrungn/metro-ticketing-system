namespace NotificationService.Application.Mails.Queries.SendCreateOrder;

public class OrderMailData
{
    public string Email { get; init; } = null!;
    public decimal Amount { get; init; }
    public List<OrderDetailMailData> OrderDetails { get; init; } = null!;
}
