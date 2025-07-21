using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Events.Routes;
using CatalogService.Domain.Entities;

namespace CatalogService.Application.Routes.DTOs;
public class RouteStationRouteReadModel : BaseReadModel
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? ThumbnailImageUrl { get; set; }
    public double LengthInKm { get; set; }

    public ICollection<StationRouteEvent> StationRoutes { get; set; } = new List<StationRouteEvent>();

}
public class StationRouteReadModel : BaseReadModel {

    public Guid StationId { get; set; }

    public Guid RouteId { get; set; }

    public int Order { get; set; }

    public double DistanceToNext { get; set; }

}
