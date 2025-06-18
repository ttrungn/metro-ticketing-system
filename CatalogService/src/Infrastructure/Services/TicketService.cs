using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Tickets.DTO;
using CatalogService.Domain.Entities;

namespace CatalogService.Infrastructure.Services;
public class TicketService : ITicketService
{
    private readonly IUnitOfWork _unitOfWork;

    public TicketService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> CreateAsync(TicketDto ticketDto, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Ticket, Guid>();

        var id = Guid.NewGuid();

        var ticket = ticketDto.ToTicket();
        ticket.Id = id;
        ticket.CreatedAt = DateTime.UtcNow;

        await repo.AddAsync(ticket, cancellationToken);

        await repo.SaveChangesAsync(cancellationToken);

        return id;
    }

    public async Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Ticket, Guid>();

        var ticket = await repo.GetByIdAsync(id, cancellationToken);

        if (ticket == null)
        {
            return Guid.Empty;
        }
        ticket.DeleteFlag = true;
        ticket.DeletedAt = DateTime.UtcNow;
        

        await repo.RemoveAsync(ticket, cancellationToken);
        await repo.SaveChangesAsync(cancellationToken);
        return id;
    }

    public async Task<IEnumerable<TicketDto>> GetAllActiveTicketAsync(CancellationToken cancellationToken = default)
    {
       var repo = _unitOfWork.GetRepository<Ticket, Guid>();

        var tickets =  repo.Query().Where(t => t.DeleteFlag == false).OrderBy(t => t.TicketType).ToList();

        var ticketDtos = tickets.Select(t => t.ToTicketDto());
        
        await repo.SaveChangesAsync(cancellationToken);
        return ticketDtos;

    }

    public async Task<TicketDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Ticket, Guid>();
        var ticket = await repo.GetByIdAsync(id, cancellationToken);

        if(ticket == null || ticket.DeleteFlag == false)
        {
            return null;
        }
        return ticket.ToTicketDto();

    }

    public async Task<Guid> UpdateAsync(TicketDto ticketDto, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Ticket, Guid>();
        if (ticketDto == null || !ticketDto.Id.HasValue)
        {
            return Guid.Empty;
        }

        var ticket = await repo.GetByIdAsync(ticketDto.Id.Value, cancellationToken);

        if (ticket == null || ticket.DeleteFlag == false)
        {
            return Guid.Empty;
        }

        ticket = ticketDto.ToTicket();

        await repo.UpdateAsync(ticket, cancellationToken);

        await repo.SaveChangesAsync(cancellationToken); 
        return ticket.Id;
    }
}
