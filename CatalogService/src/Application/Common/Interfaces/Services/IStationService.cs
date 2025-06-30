using CatalogService.Application.Routes.DTOs;
using CatalogService.Application.Stations.Commands.CreateStation;
using CatalogService.Application.Stations.Commands.UpdateStation;
using CatalogService.Application.Stations.DTOs;
using CatalogService.Application.Stations.Queries.GetStations;

namespace CatalogService.Application.Common.Interfaces.Services;

public interface IStationService
{
    Task<Guid> CreateAsync(CreateStationCommand command, CancellationToken cancellationToken = default);
    Task<Guid> UpdateAsync(UpdateStationCommand command, CancellationToken cancellationToken);
    Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<(IEnumerable<StationReadModel>, int)> GetAsync(GetStationsQuery request, CancellationToken cancellationToken = default);
    Task<StationReadModel?> GetByIdAsync(Guid queryId, CancellationToken cancellationToken = default);


    Task<StationListResponseDto> GetAllActiveStationsByName(string name, CancellationToken cancellationToken = default);


    Task<IEnumerable<SingleUseGetStationsResponseDto>> GetStationsByRouteId(Guid routeId, CancellationToken cancellationToken = default);
}
