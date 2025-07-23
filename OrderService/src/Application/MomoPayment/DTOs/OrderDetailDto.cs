using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.MomoPayment.DTOs;
public class OrderDetailDto
{
    public Guid TicketId { get; set; }

    public Guid? EntryStationId { get; set; }
    
    public Guid? DestinationStationId { get; set; }

    public decimal BoughtPrice { get; set; } = 0;
   
    public int Quantity { get; set; }

}
