using System.Linq.Expressions;
using BuildingBlocks.Domain.Events.Buses;
using BuildingBlocks.Domain.Events.Stations;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Routes.DTOs;
using CatalogService.Application.Stations.Commands.CreateStation;
using CatalogService.Application.Stations.Commands.UpdateStation;
using CatalogService.Application.Stations.DTOs;
using CatalogService.Application.Stations.Queries.GetStations;
using CatalogService.Domain.Entities;
using Marten;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CatalogService.Infrastructure.Services;

public class StationService : IStationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAzureBlobService _azureBlobService;
    private readonly IConfiguration _configuration;

    public StationService(IUnitOfWork unitOfWork, IAzureBlobService azureBlobService, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _azureBlobService = azureBlobService;
        _configuration = configuration;
    }

    public async Task<Guid> CreateAsync(
        CreateStationCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();

        var repo = _unitOfWork.GetRepository<Station, Guid>();

        var count = repo.Query().Count();
        var code = GenerateCode(count);

        var thumbnailImageUrl = "empty";
        if (command.ThumbnailImageStream != null && command.ThumbnailImageFileName != null)
        {
            var blobName = id + GetFileType(command.ThumbnailImageFileName);
            var containerName = _configuration["Azure:BlobStorageSettings:StationImagesContainerName"] ?? "station-images";
            var blobUrl = await _azureBlobService.UploadAsync(
                command.ThumbnailImageStream,
                blobName,
                containerName);
            thumbnailImageUrl = blobUrl;
        }

        var station = new Station()
        {
            Id = id,
            Code = code,
            Name = command.Name,
            StreetNumber = command.StreetNumber,
            Street = command.Street,
            Ward = command.Ward,
            District = command.District,
            City = command.City,
            ThumbnailImageUrl = thumbnailImageUrl,
        };

        await repo.AddAsync(station, cancellationToken);

        station.AddDomainEvent(new CreateStationEvent()
        {
            Id = station.Id,
            Code = station.Code,
            Name = station.Name,
            StreetNumber = station.StreetNumber,
            Street = station.Street,
            Ward = station.Ward,
            District = station.District,
            City = station.City,
            ThumbnailImageUrl = station.ThumbnailImageUrl,
        });
        await _unitOfWork.SaveChangesAsync();

        return id;
    }

    public async Task<Guid> UpdateAsync(UpdateStationCommand command, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.GetRepository<Station, Guid>();

        var station = await repo.GetByIdAsync(command.Id, cancellationToken);
        if (station == null)
        {
            return Guid.Empty;
        }

        if (command.ThumbnailImageStream != null && command.ThumbnailImageFileName != null)
        {
            var blobName = station.Id + GetFileType(command.ThumbnailImageFileName);
            var containerName = _configuration["Azure:BlobStorageSettings:StationImagesContainerName"] ?? "station-images";
            var blobUrl = await _azureBlobService.UploadAsync(
                command.ThumbnailImageStream,
                blobName,
                containerName);
            station.ThumbnailImageUrl = blobUrl;
        }

        station.Name = command.Name;
        station.StreetNumber = command.StreetNumber;
        station.Street = command.Street;
        station.Ward = command.Ward;
        station.District = command.District;
        station.City = command.City;

        station.AddDomainEvent(new UpdateStationEvent()
        {
            Id = station.Id,
            Name = station.Name,
            StreetNumber = station.StreetNumber,
            Street = station.Street,
            Ward = station.Ward,
            District = station.District,
            City = station.City,
            ThumbnailImageUrl = station.ThumbnailImageUrl,
        });

        await repo.UpdateAsync(station, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return station.Id;
    }

    public async Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.GetRepository<Station, Guid>();

        var station = await repo.GetByIdAsync(id, cancellationToken);
        if (station == null)
        {
            return Guid.Empty;
        }

        var stationRouteRepo =
            _unitOfWork.GetRepository<StationRoute, (Guid StationId, Guid RouteId)>();

        var stationRoutes = await EntityFrameworkQueryableExtensions.ToListAsync(stationRouteRepo.Query().Where(r => r.StationId == id), cancellationToken);
        if (stationRoutes.Count != 0)
        {
            foreach (var stationRoute in stationRoutes)
            {
                stationRoute.DeleteFlag = true;
                await stationRouteRepo.UpdateAsync(stationRoute, cancellationToken);
            }
        }

        var busRepo = _unitOfWork.GetRepository<Bus, Guid>();
        var buses = await EntityFrameworkQueryableExtensions.ToListAsync(busRepo.Query().Where(b => b.StationId == id), cancellationToken);
        if (buses.Count != 0)
        {
            foreach (var bus in buses)
            {
                bus.DeleteFlag = true;
                bus.AddDomainEvent(new DeleteBusEvent()
                {
                    Id = bus.Id,
                });
                await busRepo.UpdateAsync(bus, cancellationToken);
            }
        }
        station.DeleteFlag = true;

        station.AddDomainEvent(new DeleteStationEvent()
        {
            Id = station.Id,
        });

        await repo.UpdateAsync(station, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return station.Id;
    }

    public async Task<(IEnumerable<StationReadModel>, int)> GetAsync(
        GetStationsQuery query,
        CancellationToken cancellationToken)
    {
        var session = _unitOfWork.GetDocumentSession();

        Expression<Func<StationReadModel, bool>> filter = GetFilter(query);

        var stations = await QueryableExtensions.ToListAsync(session.Query<StationReadModel>()
                .Where(filter)
                .Skip(query.Page * query.PageSize)
                .Take(query.PageSize)
                .AsNoTracking(),
            cancellationToken);

        var totalCount = await QueryableExtensions.CountAsync(session.Query<StationReadModel>()
                .Where(filter)
                .AsNoTracking(),
            cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

        return (stations, totalPages);

    }

    public async Task<StationReadModel?> GetByIdAsync(Guid queryId, CancellationToken cancellationToken = default)
    {
        var session = _unitOfWork.GetDocumentSession();

        var station = await QueryableExtensions.FirstOrDefaultAsync(session.Query<StationReadModel>()
            .Where(s => s.Id == queryId),
            cancellationToken);

        return station;
    }

    #region Helper method

    private Expression<Func<StationReadModel, bool>> GetFilter(GetStationsQuery query)
    {
        return (s) =>
            s.Name!.ToLower().Contains(query.Name!.ToLower() + "") &&
            s.DeleteFlag == query.Status;
    }

    private string GenerateCode(int count, int digits = 6)
    {
        var nextCode = count + 1;
        return nextCode.ToString($"D{digits}");
    }

    private string GetFileType(string fileName)
    {
        return Path.GetExtension(fileName);
    }

    private bool compareStationName(string searchString, string? stationName)
    {
        if(stationName  == null) return false;

        return stationName.RemoveDiacritics().Contains(searchString, StringComparison.OrdinalIgnoreCase);

    }


    #endregion



    public async Task<StationListResponseDto> GetAllActiveStationsByName(string searchString, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.GetRepository<Station, Guid>();
        StationListResponseDto response = new StationListResponseDto();



        List<Station> stationList;
        if (string.IsNullOrEmpty(searchString))
        {
            stationList = await EntityFrameworkQueryableExtensions.ToListAsync(repo.Query()
                    .Where(s => s.DeleteFlag == false), cancellationToken);

            response.Stations = stationList.Select(s => s.togGetNameStationResponseDto()).ToList();

            return response;
        }
        var normalizedSearch = searchString.Trim().RemoveDiacritics();


        var rawStations = await EntityFrameworkQueryableExtensions.ToListAsync(repo.Query()
                .Where(s => s.DeleteFlag == false && s.Name != null), cancellationToken);


        stationList = rawStations
        .Where(s => compareStationName(normalizedSearch, s.Name))
        .ToList();

        response.Stations = stationList.Select(s => s.togGetNameStationResponseDto()).ToList();

        return response;
    }

    public async Task<IEnumerable<SingleUseGetStationsResponseDto>> GetStationsByRouteId(Guid routeId, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<StationRoute, (Guid,Guid)>();

        var stationRoutes = await EntityFrameworkQueryableExtensions.ToListAsync(repo.Query()
                                         .Include(s => s.Station)
                                         .Where(st => st.DeleteFlag == false && st.RouteId == routeId), cancellationToken);
        if (stationRoutes.Count == 0)
        {
            return Enumerable.Empty<SingleUseGetStationsResponseDto>();
        }
        var response = stationRoutes.Select(st => new SingleUseGetStationsResponseDto()
        {
            Id = st.StationId,
            Name = st?.Station?.Name
        }).ToList();

        return response;
    }
}
