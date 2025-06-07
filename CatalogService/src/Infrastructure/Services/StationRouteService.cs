using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Routes.DTOs;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Services;
public class StationRouteService : IStationRouteService
{
    private readonly IUnitOfWork _unitOfWork;

    public StationRouteService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<StationRoute> GetAllStationRouteAsync(Guid routeId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> UpsertStationRouteByRouteIdAsync(Guid routeId, CancellationToken cancellationToken = default)
    {
        var routeRepo = _unitOfWork.GetRepository<Route,Guid>();

        var routeStationRepo = _unitOfWork.GetRepository<StationRoute,Guid>();

        var route = await routeRepo.Query()
            .Include(r => r.StationRoutes).FirstOrDefaultAsync(r => r.Id == routeId);

        if (route == null)
        {
            return Guid.Empty;
        }

        List<StationRoute> stationRoutes = route.StationRoutes.ToList();

        foreach(var stationRoute in stationRoutes)
        {
            stationRoute.DeleteFlag = true;
            await routeStationRepo.UpdateAsync(stationRoute);
        }


        return routeId;


    }

    public async Task InsertStationRouteListAsync(List<StationRouteDto> stationRoutesDtoList, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<StationRoute, Guid>();

        var stationRouteList =  stationRoutesDtoList.Select(dto => new StationRoute
        {
            Id = Guid.NewGuid(),
            RouteId = dto.RouteId,
            StationId = dto.StationId,
            EntryStationId = dto.EntryStationId,
            DestinationStationId = dto.DestinationStationId,
            Order = dto.Order,
            DeleteFlag = false,
            CreatedAt = DateTime.UtcNow,
        }).ToList();

        foreach (var stationRoute in stationRouteList)
        {
            await repo.AddAsync(stationRoute, cancellationToken);
        }


     
    }
}
