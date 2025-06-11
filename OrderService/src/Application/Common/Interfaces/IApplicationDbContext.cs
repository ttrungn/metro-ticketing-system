using OrderService.Domain.Entities;

namespace OrderService.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Cart> Carts { get; }
    DbSet<Order> Orders { get;  }
    DbSet<OrderDetail> OrderDetails { get; }
}
