using BuildingBlocks.Domain.Common;
using Marten;
using Microsoft.EntityFrameworkCore.Storage;

namespace CatalogService.Application.Common.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T, TId> GetRepository<T, TId>()
        where T : BaseEntity<TId>
        where TId : notnull;
    
    IDocumentSession GetDocumentSession();
    
    Task<int> SaveChangesAsync();

  
}
