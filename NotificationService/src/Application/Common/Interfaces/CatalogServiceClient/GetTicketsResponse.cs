namespace NotificationService.Application.Common.Interfaces.CatalogServiceClient;

public class GetTicketsResponse
{
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; } = 0;
    public int PageSize { get; set; } = 8;
    public IEnumerable<TicketReadModel> Tickets { get; set; } = new List<TicketReadModel>();
}
