namespace OrderService.Application.Carts.DTOs;

public class GetCartsResponseDto
{
    public string CartId { get; set; } = string.Empty;
    public string TicketId { get; set; } = string.Empty;
    public string TicketName { get; set; } = string.Empty;
    public string EntryStationId { get; set; } = string.Empty;
    public string EntryStationName { get; set; } = string.Empty;
    public string DestinationStationId { get; set; } = string.Empty;
    public string DestinationStationName { get; set; } = string.Empty;
    public string Route { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    
    
}
