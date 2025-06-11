using CatalogService.Application.Routes.Commands.CreateRoute;
using CatalogService.Application.Routes.Commands.UpdateRoute;
using CatalogService.Application.Routes.DTOs;
using CatalogService.Application.Routes.Queries.GetRoutes;

namespace CatalogService.Application.Common.Interfaces.Services;

public interface IRouteService
{
    Task<Guid> CreateAsync(CreateRouteCommand command, CancellationToken cancellationToken = default);
    Task<Guid> UpdateAsync(UpdateRouteCommand command, CancellationToken cancellationToken = default);
    Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<RoutesResponseDto>, int)> GetAsync(GetRoutesQuery query, int sizePerPage, CancellationToken cancellationToken = default);
    Task<StationRouteResponseDto?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default);
}
