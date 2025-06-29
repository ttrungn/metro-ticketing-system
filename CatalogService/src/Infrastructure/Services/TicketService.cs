using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Tickets.DTO;
using CatalogService.Application.Tickets.Queries.GetSingleUseTicketWithPrice;
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

        if(ticket == null || ticket.DeleteFlag == true)
        {
            return null;
        }
        return ticket.ToTicketDto();

    }

    public Task<double> GetSingleTicketPrice(Guid routeId, Guid entryId, Guid exitId, CancellationToken cancellationToken = default)
    {
        var stationRouteRepo = _unitOfWork.GetRepository<StationRoute, (Guid, Guid)>();

        var priceRangeRepo = _unitOfWork.GetRepository<PriceRange, Guid>(); 

        var priceRangeList = priceRangeRepo.Query().Where(pr => pr.DeleteFlag == false).OrderBy(pr => pr.FromKm).ToList();

        var entryOrder = stationRouteRepo.Query().Where(es => es.DeleteFlag == false && es.StationId == entryId && es.RouteId == routeId).Select(es =>  es.Order).FirstOrDefault();

        var exitOrder = stationRouteRepo.Query().Where(es => es.DeleteFlag == false && es.StationId == exitId && es.RouteId == routeId).Select(es=> es.Order).FirstOrDefault();

        if (entryOrder == 0 || exitOrder == 0)
            return Task.FromResult(0.0);

        var minOrder = Math.Min(entryOrder, exitOrder);
        var maxOrder = Math.Max(entryOrder, exitOrder);


        var sumDistance = stationRouteRepo.Query()
                                          .Where(es => (es.DeleteFlag == false && es.RouteId == routeId) &&
                                                        (es.Order >= minOrder && es.Order < maxOrder))
                                          .Select(es => es.DistanceToNext).Sum();

        double result = 0.0;

        foreach (var pricePrange in priceRangeList)
        {
            var rangeDistance = pricePrange.ToKm - pricePrange.FromKm;

            if (sumDistance >= rangeDistance)
            {
                result = rangeDistance * (double)(pricePrange.Price);
            }else
            {
                result = sumDistance * (double)(pricePrange.Price);
            }
                sumDistance -= rangeDistance;
            if (sumDistance <= 0) break;
        }

        return Task.FromResult(RoundUpToNearest(result));
    }

    public async Task<SingleUseTicketResponseDto> GetSingleUseTicketInfo(GetSingleUseTicketWithPriceQuery request, CancellationToken cancellationToken = default)
    { 
        var ticketRepo = _unitOfWork.GetRepository<Ticket, Guid>();

        var singleUseTicket = ticketRepo.Query().FirstOrDefault(t => t.TicketType == Domain.Enum.TicketTypeEnum.SingleUseType);

        var ticketPrice = await this.GetSingleTicketPrice(request.RouteId, request.EntryStationId, request.ExitStationId);

        return new SingleUseTicketResponseDto
        {
            Id = singleUseTicket!.Id,
            Name = singleUseTicket.Name,
            entryStationId = request.EntryStationId,
            exitStationId = request.ExitStationId,
            RouteId = request.RouteId,
            Price = ticketPrice,
            ExpireInDays = singleUseTicket.ExpirationInDay,
        };

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
    private double RoundUpToNearest(double value)
    {
        if (value <= 0) return 0;
        if (value < 100000)
        {
            return Math.Ceiling(value / 1000.0) * 1000;
        }
        
        return Math.Ceiling(value / 10000.0) * 10000;
        
    }

}
