using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Application.Tickets.DTO;
public class SingleUseTicketResponseDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public double Price { get; set; }   

    public int ExpireInDays { get; set; }  
    
    public Guid entryStationId { get; set; }

    public Guid exitStationId { get; set; }

    public Guid RouteId { get; set; }   

}
