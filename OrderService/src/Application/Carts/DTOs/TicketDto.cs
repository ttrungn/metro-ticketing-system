namespace OrderService.Application.Carts.DTOs;

public class TicketDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public int ExpirationInDay { get; set; }
    
    public decimal Price { get; set; }
    
    public int TicketTypeEnum  { get; set; }

    public int ActiveInDay { get; set; }
}
