namespace OrderService.Application.Carts.DTOs;

public class CartResponseDto
{
    public string CartId { get; set; } = string.Empty;
    public string TicketName { get; set; } = string.Empty;
    public string EntryStationName { get; set; } = string.Empty;
    public string DestinationStationName { get; set; } = string.Empty;
    public string Route { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    
    
}
