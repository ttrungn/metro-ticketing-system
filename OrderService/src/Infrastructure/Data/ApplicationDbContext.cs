using System.Reflection;
using OrderService.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

    public DbSet<TransactionHistory> TransactionHistories => Set<TransactionHistory>();
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

}
