using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Application.Tickets.Commands.CreateTicket;
using CatalogService.Application.Tickets.Commands.UpdateTicket;
using CatalogService.Application.Tickets.DTO;
using CatalogService.Application.Tickets.Queries.GetSingleUseTicketWithPrice;
using CatalogService.Application.Tickets.Queries.GetTickets;

namespace CatalogService.Application.Common.Interfaces.Services;
public interface ITicketService
{
    Task<Guid> CreateAsync(TicketDto ticket, CancellationToken cancellationToken = default);

    Task<IEnumerable<TicketDto>> GetAllActiveTicketAsync(CancellationToken cancellationToken = default);


    //Task<List<TicketDto>> GetAllPagingAsync(int page, int pageSize, string filter, CancellationToken cancellationToken = default);

    Task<TicketReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Guid> UpdateAsync(TicketDto ticket, CancellationToken cancellationToken = default);

    Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken = default);


    Task<SingleUseTicketResponseDto> GetSingleUseTicketInfo(GetSingleUseTicketWithPriceQuery request , CancellationToken cancellationToken = default);


    Task<double> GetSingleTicketPrice(Guid routeId, Guid entryId, Guid exitId, CancellationToken cancellationToken = default);

    Task<Guid> CreateTicketAsync(CreateTicketCommand request, CancellationToken cancellationToken = default);
    Task<Guid> UpdateTicket(UpdateTicketCommand request, CancellationToken cancellationToken = default);
    Task<Guid> DeleteTicket(Guid requestId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<TicketReadModel>, int)> GetTickets(GetTicketsQuery query, CancellationToken cancellationToken);
}
