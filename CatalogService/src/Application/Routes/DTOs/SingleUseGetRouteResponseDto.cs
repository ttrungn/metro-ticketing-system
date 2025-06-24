using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Application.Routes.DTOs;
public class SingleUseGetRouteResponseDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }
}


public class SingleUseGetStationsResponseDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }
}
