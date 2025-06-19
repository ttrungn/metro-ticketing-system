using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Domain.Enum;

namespace CatalogService.Application.Tickets.DTO;
public class TicketDto
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public int ExpirationInDay { get; set; }
    public decimal Price { get; set; }
    public TicketTypeEnum TicketType { get; set; }

    public int ActiveInDay { get; set; }
}
