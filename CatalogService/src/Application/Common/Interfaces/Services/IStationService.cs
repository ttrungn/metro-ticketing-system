using CatalogService.Application.Stations.Commands.CreateStation;
using CatalogService.Application.Stations.Commands.UpdateStation;
using CatalogService.Application.Stations.DTOs;
using CatalogService.Application.Stations.Queries.GetStations;
using CatalogService.Domain.Entities;

namespace CatalogService.Application.Common.Interfaces.Services;

public interface IStationService
{
    Task<Guid> CreateAsync(CreateStationCommand command, CancellationToken cancellationToken = default);
    Task<Guid> UpdateAsync(UpdateStationCommand command, CancellationToken cancellationToken);
    Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<(IEnumerable<StationsResponseDto>, int)> GetAsync(GetStationsQuery request, int defaultPageSize, CancellationToken cancellationToken = default);
    Task<StationsResponseDto?> GetByIdAsync(Guid queryId, CancellationToken cancellationToken = default);


    Task<StationListResponseDto> GetAllActiveStationsByName(string name, CancellationToken cancellationToken = default);

}
