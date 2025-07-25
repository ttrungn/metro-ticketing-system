﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Application.Routes.DTOs;
using CatalogService.Domain.Entities;

namespace CatalogService.Application.Common.Mappings;
public static class StationRouteMappingExtensions
{
    public static StationRoute ToStationRoute(this Routes.DTOs.StationRouteDto stationRouteDto)
    {
        return new StationRoute
        {
            RouteId = stationRouteDto.RouteId,
            StationId = stationRouteDto.StationId,
            Order = stationRouteDto.Order,
            DistanceToNext = stationRouteDto.DistanceToNext,
        };
       
    }




    
}
