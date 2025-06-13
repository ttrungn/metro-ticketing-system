using System.Collections;
using BuildingBlocks.Domain.Common;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Infrastructure.Data;
using Marten;
using Microsoft.EntityFrameworkCore.Storage;

namespace CatalogService.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IDocumentSession _session;
    private readonly Hashtable _repos = new();

    public UnitOfWork(ApplicationDbContext context, IDocumentSession session)
    {
        _context = context;
        _session = session;
    }

    public IGenericRepository<T, TId> GetRepository<T, TId>()
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

    public IDocumentSession GetDocumentSession() => _session;
    
    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public void Dispose() =>
        _context.Dispose();

}
