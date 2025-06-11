using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Application.Routes.Commands.UpsertStationRoute;
using CatalogService.Application.Routes.DTOs;
using CatalogService.Domain.Entities;

namespace CatalogService.Application.Common.Mappings;
public static class StationRouteMappingExtensions
{
    public static StationRoute ToStationRoute(this StationRouteDto stationRouteDto)
    {
        return new StationRoute
        {
            RouteId = stationRouteDto.RouteId,
            StationId = stationRouteDto.StationId,
            EntryStationId = stationRouteDto.EntryStationId,
            DestinationStationId = stationRouteDto.DestinationStationId,
            Order = stationRouteDto.Order,
            Length = stationRouteDto.Length,
        };
    }

    
}
