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

   
}
