using BuildingBlocks.Domain.Common;

namespace UserService.Application.Common.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T, TId> Repository<T, TId>()
        where T : BaseEntity<TId>
        where TId : notnull;
    
    Task<int> SaveChangesAsync();
}
