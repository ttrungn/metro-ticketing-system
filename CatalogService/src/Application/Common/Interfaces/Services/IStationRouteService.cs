using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Application.Routes.DTOs;
using CatalogService.Domain.Entities;

namespace CatalogService.Application.Common.Interfaces.Services;
public interface IStationRouteService
{
    Task<StationRoute> GetAllActiveStationRouteByRouteIdAsync(Guid routeId, CancellationToken cancellationToken = default);

}
