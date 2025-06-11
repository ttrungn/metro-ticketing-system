using System.Linq.Expressions;
using CatalogService.Application.Buses.Commands.CreateBus;
using CatalogService.Application.Buses.Commands.UpdateBus;
using CatalogService.Application.Buses.DTOs;
using CatalogService.Application.Buses.Queries.GetBuses;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Domain.Entities;

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
            StationId = command.StationId,
            DestinationName = command.DestinationName,
        };

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

        if (command.StationId.HasValue)
        {
            var station = await stationRepo.GetByIdAsync(command.StationId.Value, cancellationToken);
            if (station == null)
            {
                return Guid.Empty;
            }
            bus.StationId = command.StationId.Value;
        }

        bus.DestinationName = command.DestinationName;

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
        await repo.UpdateAsync(bus, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return bus.Id;
    }

    public async Task<BusResponseDto?> GetByIdAsync(Guid queryId, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.GetRepository<Bus, Guid>();
        return await repo.GetByIdAsync(queryId, cancellationToken)
            .ContinueWith(task =>
            {
                var bus = task.Result;
                if (bus == null)
                {
                    return null;
                }
                return new BusResponseDto
                {
                    Id = bus.Id,
                    Code = bus.Code,
                    StationId = bus.StationId,
                    DestinationName = bus.DestinationName
                };
            }, cancellationToken);
    }

    public async Task<(IEnumerable<BusResponseDto>, int)> GetAsync(
        GetBusesQuery request,
        int sizePerPage,
        CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.GetRepository<Bus, Guid>();

        Expression<Func<Bus, bool>> filter = GetFilter(request);

        var buses = await repo.GetPagedAsync(
            skip: request.Page * sizePerPage,
            take: sizePerPage,
            filters: [filter],
            cancellationToken: cancellationToken);

        var totalPages = await repo.GetTotalPagesAsync(sizePerPage, [filter], cancellationToken);

        return (
            buses.Select(b => new BusResponseDto
            {
                Id = b.Id,
                Code = b.Code,
                StationId = b.StationId,
                DestinationName = b.DestinationName
            }), totalPages);
    }

    private Expression<Func<Bus, bool>> GetFilter(GetBusesQuery query)
    {
        return (b) =>
            (query.StationId == Guid.Empty || b.StationId == query.StationId) &&
            b.DestinationName!.ToLower().Contains(query.DestinationName!.ToLower() + "") &&
            b.DeleteFlag == query.Status;
    }

    private string GenerateCode(int count, int digits = 6)
    {
        var nextCode = count + 1;
        return nextCode.ToString($"D{digits}");
    }
}
