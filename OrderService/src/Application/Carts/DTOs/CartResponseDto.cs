namespace OrderService.Application.Carts.DTOs;

public class CartResponseDto
{
    public string Ticket { get; set; } = string.Empty;
    public string EntryStationId { get; set; } = string.Empty;
    public string DestinationStationId { get; set; } = string.Empty;
    public string Route { get; set; } = null!;
    public int Quantity { get; set; }
    
}
