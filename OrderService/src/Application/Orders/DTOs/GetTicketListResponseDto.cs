using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Orders.DTOs;
public class GetTicketListResponseDto
{
    public List<TicketDto> Tickets { get; set; } = new List<TicketDto>();
}
