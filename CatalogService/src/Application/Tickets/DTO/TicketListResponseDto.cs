using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Application.Tickets.DTO;

namespace CatalogService.Application.Ticket.DTO;
public class TicketListResponseDto
{
    public List<TicketDto> Tickets { get; set; } = new List<TicketDto>();
}
