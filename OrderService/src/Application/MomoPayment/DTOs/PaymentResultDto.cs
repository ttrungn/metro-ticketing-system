using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.MomoPayment.DTOs;
public class PaymentResultDto
{
    public bool IsConfirm { get; set; }
    public int TicketCount { get; set; }
    public string Message { get; set; } = string.Empty;
}
