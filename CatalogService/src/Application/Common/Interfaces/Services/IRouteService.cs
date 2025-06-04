using CatalogService.Application.Routes.Commands.CreateRoute;
using CatalogService.Application.Routes.Commands.UpdateRoute;

namespace CatalogService.Application.Common.Interfaces.Services;

public interface IRouteService
{
    Task<Guid> CreateAsync(CreateRouteCommand command, CancellationToken cancellationToken = default);
    Task<Guid> UpdateAsync(UpdateRouteCommand command, CancellationToken cancellationToken = default);
    Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
