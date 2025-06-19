using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Application.Tickets.DTO;
using CatalogService.Domain.Enum;

namespace CatalogService.Application.Common.Mappings;
public static class TicketMappingExtensions
{
    public static CatalogService.Domain.Entities.Ticket ToTicket(this TicketDto ticketDto)
    { 
        return new CatalogService.Domain.Entities.Ticket
        {
            Name = ticketDto.Name,
            ExpirationInDay = ticketDto.ExpirationInDay,
            Price = ticketDto.Price,
            TicketType = ticketDto.TicketType,
            ActiveInDay = ticketDto.ActiveInDay
        };
    }


    public static TicketDto ToTicketDto(this CatalogService.Domain.Entities.Ticket ticket)
    {
        return new TicketDto
        {
            Id = ticket.Id,
            Name = ticket.Name,
            ExpirationInDay = ticket.ExpirationInDay,
            Price = ticket.Price,
            TicketType = ticket.TicketType,
            ActiveInDay = ticket.ActiveInDay
        };
    }
}
