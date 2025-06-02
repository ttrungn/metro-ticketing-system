using System.Collections;
using BuildingBlocks.Domain.Common;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories;

public class UnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Hashtable _repos = new();

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<T, TId> Repository<T, TId>()
        where T : BaseEntity<TId>
        where TId : notnull
    {
        var typeName = typeof(T).Name;
        if (_repos.ContainsKey(typeName))
            return (IGenericRepository<T, TId>)_repos[typeName]!;

        var repoInstance = new GenericRepository<T, TId>(_context);
        _repos.Add(typeName, repoInstance);
        return repoInstance;
    }

    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public void Dispose() =>
        _context.Dispose();
}
