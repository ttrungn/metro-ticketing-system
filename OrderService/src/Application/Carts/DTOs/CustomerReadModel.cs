using BuildingBlocks.Domain.Common;

namespace OrderService.Application.Carts.DTOs;

public class CustomerReadModel: BaseReadModel
{
    public string Id { get; set; } = null!;
    // public string FullName { get; set; } = null!;
    public string? Email { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public bool IsStudent { get; set; } = false;
}
