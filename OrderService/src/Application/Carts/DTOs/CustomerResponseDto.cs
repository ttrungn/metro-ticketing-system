namespace OrderService.Application.Carts.DTOs;

public class CustomerResponseDto
{
    public string CustomerId { get; set; } = null!;
    public string? Email { get; set; } = null!;
    public bool IsStudent { get; set; } = false;
}
