namespace NotificationService.Application.Mails.Queries.SendCreateOrder;

public class OrderDetailRequestDto
{
    public Guid TicketId { get; init; }
    public Guid EntryStationId { get; init; }
    public Guid DestinationStationId { get; init; } 
    public decimal Price { get; init; }
    public int Quantity { get; init; }
}
