namespace OrderService.Application.Carts.DTOs;

public class GetTicketInfoRequestDto
{
    public Guid RouteId { get; set; }
    public Guid EntryStationId { get; set; }
    public Guid ExitStationId { get; set; }
}
