using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Application.Stations.DTOs;
public class StationListResponseDto
{
   public  List<GetByNameStationResponseDto>? Stations { get; set; }
}
