using CatalogService.Application.Routes.Commands.CreateRoute;
using CatalogService.Application.Routes.Commands.UpdateRoute;
using CatalogService.Application.Routes.Commands.UpsertRouteStation;
using CatalogService.Application.Routes.DTOs;
using CatalogService.Application.Routes.Queries.GetRoutes;
using CatalogService.Application.Tickets.DTO;

namespace CatalogService.Application.Common.Interfaces.Services;

public interface IRouteService
{
    Task<Guid> CreateAsync(CreateRouteCommand command, CancellationToken cancellationToken = default);
    Task<Guid> UpdateAsync(UpdateRouteCommand command, CancellationToken cancellationToken = default);
    Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<RoutesResponseDto>, int)> GetAsync(GetRoutesQuery query, CancellationToken cancellationToken = default);

    Task<StationRouteResponseDto?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default);

    Task<Guid> UpsertRouteStationAsync(UpsertStationRouteCommand command, CancellationToken cancellationToken = default);


    Task<IEnumerable<SingleUseGetRouteResponseDto>> GetSingleUseRoutesAsync(CancellationToken cancellationToken = default);


}
