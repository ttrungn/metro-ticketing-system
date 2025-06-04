using OrderService.Domain.Entities;

namespace OrderService.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<PurchasedTicket> PurchasedTickets { get; }
    DbSet<Cart> Carts { get; }
    DbSet<Order> Orders { get;  }
    DbSet<OrderDetail> OrderDetails { get; }
}
