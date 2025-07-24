namespace NotificationService.Application.Mails.Queries.SendCreateOrder;

public class OrderDetailMailData
{
    public string TicketName { get; init; } = null!;
    public string EntryStationName { get; init; } = null!;
    public string DestinationStationName { get; init; } = null!; 
    public decimal Price { get; init; }
    public int Quantity { get; init; }
}
