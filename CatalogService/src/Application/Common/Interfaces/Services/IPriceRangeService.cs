using CatalogService.Application.PriceRanges.Commands;
using CatalogService.Application.PriceRanges.DTOs;
using CatalogService.Application.PriceRanges.Queries;

namespace CatalogService.Application.Common.Interfaces.Services;

public interface IPriceRangeService
{
    Task<Guid> CreateAsync(CreatePriceRangeCommand command, CancellationToken cancellationToken = default);
    Task<(IEnumerable<PriceRangeDto>, int)> GetAsync(GetPriceRangesQuery request, CancellationToken cancellationToken = default);
    Task<Guid> UpdateAsync(UpdatePriceRangeCommand command, CancellationToken cancellationToken = default);
    Task<Guid> DeleteAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<PriceRangeDto?> GetByIdAsync(Guid queryId, CancellationToken cancellationToken);
}
