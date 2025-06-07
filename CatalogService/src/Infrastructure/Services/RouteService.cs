using System.Linq.Expressions;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Routes.Commands.CreateRoute;
using CatalogService.Application.Routes.Commands.UpdateRoute;
using CatalogService.Application.Routes.DTOs;
using CatalogService.Application.Routes.Queries.GetRoutes;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Services;

public class RouteService : IRouteService
{
    private readonly IUnitOfWork _unitOfWork;

    public RouteService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> CreateAsync(CreateRouteCommand command,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Route, Guid>();

        var availableRoute = await GetRouteByCodeAsync(command.Code, cancellationToken);
        if (availableRoute != null)
        {
            return Guid.Empty;
        }

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

    public async Task<Guid> UpdateAsync(UpdateRouteCommand command,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Route, Guid>();

        var route = await repo.GetByIdAsync(command.Id, cancellationToken);
        if (route == null)
        {
            return Guid.Empty;
        }

        var availableRoute = await GetRouteByCodeAsync(command.Code, cancellationToken);
        if (availableRoute != null  && availableRoute.Id != command.Id)
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

    public async Task<(IEnumerable<RoutesResponseDto>, int)> GetAsync(
        GetRoutesQuery query,
        int sizePerPage,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Route, Guid>();

        Expression<Func<Route, bool>> filter = GetFilter(query);

        var routes = await repo.GetPagedAsync(
            skip: query.Page * sizePerPage,
            take: sizePerPage,
            filters: [filter],
            cancellationToken: cancellationToken);

        var totalPages = await repo.GetTotalPagesAsync(sizePerPage, [filter], cancellationToken);

        return (
            routes.Select(r => new RoutesResponseDto
            {
                Id = r.Id,
                Code = r.Code,
                Name = r.Name,
                ThumbnailImageUrl = r.ThumbnailImageUrl,
                LengthInKm = r.LengthInKm
            }), totalPages);
    }

    public Task<RoutesResponseDto?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Route, Guid>();

        return repo.GetByIdAsync(requestId, cancellationToken)
            .ContinueWith(task =>
            {
                var route = task.Result;
                if (route == null) return null;

                return new RoutesResponseDto
                {
                    Id = route.Id,
                    Code = route.Code,
                    Name = route.Name,
                    ThumbnailImageUrl = route.ThumbnailImageUrl,
                    LengthInKm = route.LengthInKm
                };
            }, cancellationToken);
    }

    private async Task<Route?> GetRouteByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.GetRepository<Route, Guid>();
        return await repo.Query().FirstOrDefaultAsync(r => r.Code == code, cancellationToken);
    }

    #region Helper method

    private Expression<Func<Route, bool>> GetFilter(GetRoutesQuery query)
    {
        return (r) =>
            r.Name!.ToLower().Contains(query.Name!.ToLower() + "");
    }

    #endregion
}
