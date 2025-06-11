using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Domain.Entities;

namespace CatalogService.Application.Routes.Commands.UpsertStationRoute;
public class UpsertStationRouteRequest
{
    public RouteRequestDto? Route { get; set; }
}


public class RouteRequestDto
{
    public Guid RouteId { get; set; }

    public IEnumerable<StationRouteRequestDto>? StationRoutes { get; set; }
}

public class StationRouteRequestDto
{
    public Guid StationId { get; set; }

    public Guid RouteId { get; set; }

    public int Order { get; set; }

    public double Length { get; set; }
}
