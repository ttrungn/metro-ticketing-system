using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Application.Stations.DTOs;
public class GetByNameStationResponseDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public string? Code { get; set; }
}
