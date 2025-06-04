using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Routes.Commands.CreateRoute;
using CatalogService.Application.Routes.Commands.UpdateRoute;
using CatalogService.Domain.Entities;

namespace CatalogService.Infrastructure.Services;

public class RouteService : IRouteService
{
    private readonly IUnitOfWork _unitOfWork;

    public RouteService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> CreateAsync(CreateRouteCommand command, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Route, Guid>();

        var routeId = Guid.NewGuid();


        var newRoute = new Route()
        {
            Id = routeId,
            Code = command.Code,
            Name = command.Name,
            ThumbnailImageUrl = command.ThumbnailImageUrl,
            LengthInKm = command.LengthInKm,
        };

        await repo.AddAsync(newRoute, cancellationToken);

        await _unitOfWork.SaveChangesAsync();

        return routeId;
    }

    public async Task<Guid> UpdateAsync(UpdateRouteCommand command, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Route, Guid>();


        var route = await repo.GetByIdAsync(command.Id, cancellationToken);

        if (route == null)
        {
            return Guid.Empty;
        }

        route.Code = command.Code;
        route.Name = command.Name;
        route.ThumbnailImageUrl = command.ThumbnailImageUrl;
        route.LengthInKm = command.LengthInKm;

        await repo.UpdateAsync(route, cancellationToken);

        await _unitOfWork.SaveChangesAsync();

        return route.Id;
    }

    public async Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Route, Guid>();

        var route = await repo.GetByIdAsync(id, cancellationToken);

        if (route == null)
        {
            return Guid.Empty;

        }

        route.DeleteFlag = true;

        await repo.UpdateAsync(route, cancellationToken);

        await _unitOfWork.SaveChangesAsync();

        return route.Id;
    }
}
