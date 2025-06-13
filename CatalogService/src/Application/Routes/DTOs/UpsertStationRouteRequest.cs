using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Domain.Entities;

namespace CatalogService.Application.Routes.DTOs;
public class UpsertStationRouteRequest
{
    public RouteRequestDto? Route { get; set; }
}


public class RouteRequestDto
{
    public Guid RouteId { get; set; }

    public IEnumerable<StationRouteDto>? StationRoutes { get; set; }
}


