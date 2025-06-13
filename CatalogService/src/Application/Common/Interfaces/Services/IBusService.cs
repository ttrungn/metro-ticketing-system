using CatalogService.Application.Buses.Commands.CreateBus;
using CatalogService.Application.Buses.Commands.UpdateBus;
using CatalogService.Application.Buses.DTOs;
using CatalogService.Application.Buses.Queries.GetBuses;

namespace CatalogService.Application.Common.Interfaces.Services;

public interface IBusService
{
    Task<Guid> CreateAsync(CreateBusCommand command, CancellationToken cancellationToken = default);
    Task<Guid> UpdateAsync(UpdateBusCommand command, CancellationToken cancellationToken = default);
    Task<Guid> DeleteAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<BusResponseDto?> GetByIdAsync(Guid queryId, CancellationToken cancellationToken);
    Task<(IEnumerable<BusResponseDto>, int)> GetAsync(GetBusesQuery request, CancellationToken cancellationToken);
}
