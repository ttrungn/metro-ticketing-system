using System.Linq.Expressions;
using BuildingBlocks.Domain.Events.Buses;
using CatalogService.Application.Buses.Commands.CreateBus;
using CatalogService.Application.Buses.Commands.UpdateBus;
using CatalogService.Application.Buses.DTOs;
using CatalogService.Application.Buses.Queries.GetBuses;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Domain.Entities;
using Marten;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Services;

public class BusService : IBusService
{
    private readonly IUnitOfWork _unitOfWork;

    public BusService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> CreateAsync(CreateBusCommand command, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Bus, Guid>();
        var stationRepo = _unitOfWork.GetRepository<Station, Guid>();
        var station = await stationRepo.GetByIdAsync(command.StationId, cancellationToken);
        if (station == null)
        {
            return Guid.Empty;
        }

        var count = repo.Query().Count();
        var code = GenerateCode(count);

        var id = Guid.NewGuid();

        var bus = new Bus()
        {
            Id = id,
            Code = code,
            StationId = station.Id,
            DestinationName = command.DestinationName,
        };

        bus.AddDomainEvent(new CreateBusEvent()
        {
            Id = bus.Id,
            Code = bus.Code,
            StationId = bus.StationId,
            StationName = station.Name,
            DestinationName = bus.DestinationName,
        });

        await repo.AddAsync(bus, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
        return id;
    }

    public async Task<Guid> UpdateAsync(UpdateBusCommand command, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Bus, Guid>();
        var stationRepo = _unitOfWork.GetRepository<Station, Guid>();

        var bus = await repo.GetByIdAsync(command.Id, cancellationToken);
        if (bus == null)
        {
            return Guid.Empty;
        }
        string stationName = "unknown";

        if (command.StationId.HasValue)
        {
            var station = await stationRepo.GetByIdAsync(command.StationId.Value, cancellationToken);
            if (station == null)
            {
                return Guid.Empty;
            }
            bus.StationId = command.StationId.Value;
            stationName = station.Name ?? "unknown";
        }

        bus.DestinationName = command.DestinationName;

        bus.AddDomainEvent(new UpdateBusEvent()
        {
            Id = bus.Id,
            StationId = bus.StationId,
            StationName = stationName,
            DestinationName = bus.DestinationName,
        });

        await repo.UpdateAsync(bus, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
        return bus.Id;
    }

    public async Task<Guid> DeleteAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Bus, Guid>();
        var bus = await repo.GetByIdAsync(requestId, cancellationToken);
        if (bus == null)
        {
            return Guid.Empty;
        }

        bus.DeleteFlag = true;

        bus.AddDomainEvent(new DeleteBusEvent()
        {
            Id = bus.Id,
        });

        await repo.UpdateAsync(bus, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return bus.Id;
    }

    public async Task<BusReadModel?> GetByIdAsync(Guid queryId, CancellationToken cancellationToken)
    {
        var session = _unitOfWork.GetDocumentSession();

        var bus = await QueryableExtensions.FirstOrDefaultAsync(session.Query<BusReadModel>()
                .Where(s => s.Id == queryId),
            cancellationToken);

        return bus;
    }

    public async Task<(IEnumerable<BusReadModel>, int)> GetAsync(
        GetBusesQuery query,
        CancellationToken cancellationToken)
    {
        var session = _unitOfWork.GetDocumentSession();

        Expression<Func<BusReadModel, bool>> filter = GetFilter(query);

        var buses = await QueryableExtensions.ToListAsync(session.Query<BusReadModel>()
                .Where(filter)
                .Skip(query.Page * query.PageSize)
                .Take(query.PageSize)
                .AsNoTracking(),
            cancellationToken);

        var totalCount = await QueryableExtensions.CountAsync(session.Query<BusReadModel>()
                .Where(filter)
                .AsNoTracking(),
            cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

        return (buses, totalPages);
    }

    private Expression<Func<BusReadModel, bool>> GetFilter(GetBusesQuery query)
    {
        return (b) =>
            (query.StationId == Guid.Empty || b.StationId == query.StationId) &&
            (query.StationName == null || b.StationName!.ToLower().Contains(query.StationName!.ToLower() + "")) &&
            b.DestinationName!.ToLower().Contains(query.DestinationName!.ToLower() + "") &&
            b.DeleteFlag == query.Status;
    }

    private string GenerateCode(int count, int digits = 6)
    {
        var nextCode = count + 1;
        return nextCode.ToString($"D{digits}");
    }
}
