using BuildingBlocks.Domain.Common;

namespace OrderService.Application.Common.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T, TId> GetRepository<T, TId>()
        where T : BaseEntity<TId>
        where TId : notnull;
    
    Task<int> SaveChangesAsync();
}
