using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Application.Routes.DTOs;
using CatalogService.Application.Stations.DTOs;
using CatalogService.Domain.Entities;

namespace CatalogService.Application.Common.Mappings;
public static class StationMappingExtensions
{
    public static GetByNameStationResponseDto togGetNameStationResponseDto(this Station st)
    {
        return new GetByNameStationResponseDto
        {
            Code = st.Code,
            Name = st.Name,
            Id = st.Id
        };
    }
}
