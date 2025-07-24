namespace BuildingBlocks.Domain.Events.Orders;

public class CreateOrderEventOrderDetail
{
    public Guid TicketId { get; init; }
    public Guid? EntryStationId { get; init; }
    public Guid? DestinationStationId { get; init; } 
    public decimal Price { get; init; }
    public int Quantity { get; init; }
}