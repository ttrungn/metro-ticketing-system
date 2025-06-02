using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Enums;

public enum PurchaseTicketStatus
{
    Unused = 0,
    Used = 1,
    Expired = 2,
}
