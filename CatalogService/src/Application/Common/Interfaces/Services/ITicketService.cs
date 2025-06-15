using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Application.Tickets.DTO;

namespace CatalogService.Application.Common.Interfaces.Services;
public interface ITicketService
{
    Task<Guid> CreateAsync(TicketDto ticket, CancellationToken cancellationToken = default);   

    Task<IEnumerable<TicketDto>> GetAllActiveTicketAsync(CancellationToken cancellationToken = default);


    //Task<List<TicketDto>> GetAllPagingAsync(int page, int pageSize, string filter, CancellationToken cancellationToken = default);
    
    Task<TicketDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Guid> UpdateAsync(TicketDto ticket, CancellationToken cancellationToken = default);    

    Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
} 
