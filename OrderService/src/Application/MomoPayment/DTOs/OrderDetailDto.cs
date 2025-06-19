using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.MomoPayment.DTOs;
public class OrderDetailDto
{
    public string TicketId { get; set; } = null!;

    public string EntryStationId { get; set; } = null!;
    
    public string DestinationStationId { get; set; } = null!;

    public decimal BoughtPrice { get; set; } = 0;

}
