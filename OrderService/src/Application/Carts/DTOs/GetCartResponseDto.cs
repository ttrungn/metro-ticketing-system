namespace OrderService.Application.Carts.DTOs;

public class GetCartResponseDto
{
    public string TicketName { get; set; } = string.Empty;
    public string EntryStationName { get; set; } = string.Empty;
    public string DestinationStationName { get; set; } = string.Empty;
    public string RouteName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
