namespace OrderService.Application.Orders.DTOs;

public class GetTicketsResponseDto
{
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; } = 0;
    public int PageSize { get; set; } = 8;
    public IEnumerable<TicketReadModel> Tickets { get; set; } = new List<TicketReadModel>();
}
