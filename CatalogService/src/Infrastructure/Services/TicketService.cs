﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Events.Tickets;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Tickets.Commands.CreateTicket;
using CatalogService.Application.Tickets.Commands.UpdateTicket;
using CatalogService.Application.Tickets.DTO;
using CatalogService.Application.Tickets.Queries.GetSingleUseTicketWithPrice;
using CatalogService.Application.Tickets.Queries.GetTickets;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Enum;
using Marten;
using Microsoft.EntityFrameworkCore;

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

    public async Task<TicketReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var session = _unitOfWork.GetDocumentSession();
        var ticket = await QueryableExtensions.FirstOrDefaultAsync(session.Query<TicketReadModel>()
            .Where(t => t.Id == id && t.DeleteFlag == false).AsNoTracking(), cancellationToken);
        return ticket;

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
            var rangeDistance = pricePrange.ToKm - pricePrange.FromKm + 1;

            if (sumDistance >= rangeDistance)
            {
                result += rangeDistance * (double)(pricePrange.Price);
            }
            else
            {
                result += sumDistance * (double)(pricePrange.Price);
            }
            sumDistance -= rangeDistance;
            if (sumDistance <= 0) break;
        }

        return Task.FromResult(RoundUpToNearest(result));
    }

    public async Task<SingleUseTicketResponseDto> GetSingleUseTicketInfo(GetSingleUseTicketWithPriceQuery request, CancellationToken cancellationToken = default)
    {
        var session = _unitOfWork.GetDocumentSession();

        var singleUseTicket = await QueryableExtensions.FirstOrDefaultAsync(session.Query<TicketReadModel>()
            .Where(t => t.TicketType == TicketTypeEnum.SingleUseType && t.DeleteFlag == false).AsNoTracking()
            , cancellationToken);
        if (singleUseTicket == null)
            throw new Exception("Không tìm thấy vé lượt đơn.");

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


    public async Task<Guid> CreateTicketAsync(CreateTicketCommand request, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Ticket, Guid>();
        var id = Guid.NewGuid();

        var ticket = new Ticket()
        {
            Id = id,
            Name = request.Name,
            Price = request.Price,
            ActiveInDay = request.ActiveInDay,
            ExpirationInDay = request.ExpirationInDay,
            TicketType = request.TicketType,
        };

        ticket.AddDomainEvent(new CreateTicketEvent()
        {
            Id = ticket.Id,
            Name = ticket.Name,
            Price = ticket.Price,
            ActiveInDay = ticket.ActiveInDay,
            ExpirationInDay = ticket.ExpirationInDay,
            TicketType = (int)ticket.TicketType,
        });

        await repo.AddAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
        return ticket.Id;
    }

    public async Task<Guid> UpdateTicket(UpdateTicketCommand request, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Ticket, Guid>();
        var ticket = await repo.GetByIdAsync(request.Id, cancellationToken);
        if (ticket == null)
        {
            return Guid.Empty;
        }

        ticket.Name = request.Name;
        ticket.Price = request.Price ?? 0;
        ticket.ActiveInDay = request.ActiveInDay ?? 0;
        ticket.ExpirationInDay = request.ExpirationInDay ?? 0;
        ticket.TicketType = request.TicketType ?? TicketTypeEnum.SingleUseType;

        ticket.AddDomainEvent(new UpdateTicketEvent()
        {
            Id = ticket.Id,
            Name = ticket.Name,
            Price = ticket.Price,
            ActiveInDay = ticket.ActiveInDay,
            ExpirationInDay = ticket.ExpirationInDay,
            TicketType = (int)ticket.TicketType,
        });

        await repo.UpdateAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return ticket.Id;
    }

    public async Task<Guid> DeleteTicket(Guid requestId, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Ticket, Guid>();
        var ticket = await repo.GetByIdAsync(requestId, cancellationToken);
        if (ticket == null)
        {
            return Guid.Empty;
        }

        ticket.DeleteFlag = true;
        ticket.AddDomainEvent(new DeleteTicketEvent() { Id = ticket.Id });

        await repo.UpdateAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return ticket.Id;
    }

    public async Task<(IEnumerable<TicketReadModel>, int)> GetTickets(GetTicketsQuery query, CancellationToken cancellationToken)
    {
        var session = _unitOfWork.GetDocumentSession();

        Expression<Func<TicketReadModel, bool>> filter = GetFilter(query);

        var tickets = await QueryableExtensions.ToListAsync(session.Query<TicketReadModel>()
                .Where(filter)
                .Skip(query.Page * query.PageSize)
                .Take(query.PageSize)
                .AsNoTracking(),
            cancellationToken);

        var totalCount = await QueryableExtensions.CountAsync(session.Query<TicketReadModel>()
                .Where(filter)
                .AsNoTracking(),
            cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

        return (tickets, totalPages);
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

    private Expression<Func<TicketReadModel, bool>> GetFilter(GetTicketsQuery query)
    {
        return r =>
            (string.IsNullOrEmpty(query.Name) || r.Name!.ToLower().Contains(query.Name.ToLower())) &&
            (query.MinPrice == null || r.Price >= query.MinPrice) &&
            (query.MaxPrice == null || r.Price <= query.MaxPrice) &&
            (query.TicketType == null || r.TicketType == query.TicketType) &&
            (query.Status == null || r.DeleteFlag == query.Status);
    }

}
