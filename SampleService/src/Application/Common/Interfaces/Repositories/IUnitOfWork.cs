using BuildingBlocks.Domain.Common;
using Marten;

namespace SampleService.Application.Common.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T, TId> GetRepository<T, TId>()
        where T : BaseEntity<TId>
        where TId : notnull;
    
    IDocumentSession GetDocumentSession();
    
    Task<int> SaveChangesAsync();
}
