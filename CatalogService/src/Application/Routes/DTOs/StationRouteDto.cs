using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Application.Routes.DTOs;
public class StationRouteDto
{
    public Guid StationId { get; set; }

    public Guid RouteId { get; set; }

    public Guid EntryStationId { get; set; }

    public Guid DestinationStationId { get; set; }

    public int Order { get; set; }

    public double Length { get; set; }
}
