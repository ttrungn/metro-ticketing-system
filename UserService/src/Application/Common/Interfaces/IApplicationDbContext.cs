using UserService.Domain.Entities;

namespace UserService.Application.Common.Interfaces;

public interface IApplicationDbContext
{   
    DbSet<Customer> Customers { get; }
    DbSet<Staff> Staffs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
