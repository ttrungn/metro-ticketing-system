using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.PriceRanges.Commands;
using CatalogService.Application.PriceRanges.DTOs;
using CatalogService.Application.PriceRanges.Queries;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Services;

public class PriceRangeService : IPriceRangeService
{
    private readonly IUnitOfWork _unitOfWork;
    public PriceRangeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> CreateAsync(CreatePriceRangeCommand command, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<PriceRange, Guid>();
        var priceRanges = await repo.Query()
            .Where(p => p.DeleteFlag == false)
            .ToListAsync(cancellationToken);

        if (priceRanges.Any())
        {
            var minFromKm = priceRanges.Min(x => x.FromKm);
            var maxToKm = priceRanges.Max(x => x.ToKm);

            if ((command.FromKm >= minFromKm && command.FromKm < maxToKm) ||
                (command.ToKm > minFromKm && command.ToKm <= maxToKm))
            {
                return Guid.Empty;
            }
        }
        var priceRange = new PriceRange()
        {
            FromKm = command.FromKm,
            ToKm = command.ToKm,
            Price = command.Price
        };

        await repo.AddAsync(priceRange, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return priceRange.Id;
    }

    public async Task<(IEnumerable<PriceRangeDto>, int)> GetAsync(GetPriceRangesQuery request, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<PriceRange, Guid>();
        var query = repo.Query();

        var totalCount = await query.CountAsync(cancellationToken);
        var priceRanges = await query
            .OrderBy(x => x.FromKm)
            .Skip(request.PageSize * request.Page)
            .Take(request.PageSize)
            .Select(x => new PriceRangeDto
            {
                Id = x.Id,
                FromKm = x.FromKm,
                ToKm = x.ToKm,
                Price = x.Price,
                DeleteFlag = x.DeleteFlag
            })
            .ToListAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);


        return (priceRanges, totalPages);
    }

    public async Task<Guid> UpdateAsync(UpdatePriceRangeCommand command, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<PriceRange, Guid>();
        var priceRange = await repo.GetByIdAsync(command.Id, cancellationToken);
        if (priceRange == null || priceRange.DeleteFlag)
        {
            return Guid.Empty;
        }

        var minFromKm = await repo.Query().Where(p=> p.DeleteFlag == false && p.Id != command.Id)
                                            .MinAsync(x => x.FromKm, cancellationToken);
        var maxToKm = await repo.Query().Where(p=> p.DeleteFlag == false && p.Id != command.Id)
                                        .MaxAsync(x => x.ToKm, cancellationToken);
        if ((command.FromKm >= minFromKm && command.FromKm < maxToKm) || (command.ToKm > minFromKm && command.ToKm <= maxToKm))
        {
            return Guid.Empty;
        }

        priceRange.FromKm = command.FromKm;
        priceRange.ToKm = command.ToKm;
        priceRange.Price = command.Price;

        await repo.UpdateAsync(priceRange, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return priceRange.Id;
    }
    public async Task<Guid> DeleteAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<PriceRange, Guid>();
        var priceRange = await repo.GetByIdAsync(requestId, cancellationToken);
        if (priceRange == null || priceRange.DeleteFlag)
        {
            return Guid.Empty;
        }
        var fromKmToDelete = priceRange.FromKm;
        var toKmToDelete = priceRange.ToKm;

        var nextPriceRange = await repo.Query()
            .Where(p => p.DeleteFlag == false && p.Id != requestId && p.ToKm == fromKmToDelete )
            .FirstOrDefaultAsync(cancellationToken);

        var prevPriceRange = await repo.Query()
            .Where(p => p.DeleteFlag == false && p.Id != requestId && p.FromKm == toKmToDelete )
            .FirstOrDefaultAsync(cancellationToken);
        if (nextPriceRange != null && prevPriceRange != null)
            return Guid.Empty;

        priceRange.DeleteFlag = true;

        await repo.UpdateAsync(priceRange, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return priceRange.Id;
    }
    public async Task<PriceRangeDto?> GetByIdAsync(Guid queryId, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.GetRepository<PriceRange, Guid>();
        var priceRange = await repo.GetByIdAsync(queryId, cancellationToken);

        return new PriceRangeDto
        {
            Id = priceRange!.Id,
            FromKm = priceRange.FromKm,
            ToKm = priceRange.ToKm,
            Price = priceRange.Price,
            DeleteFlag = priceRange.DeleteFlag
        };
    }
}
