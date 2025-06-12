using System.Linq.Expressions;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Stations.Commands.CreateStation;
using CatalogService.Application.Stations.Commands.UpdateStation;
using CatalogService.Application.Stations.DTOs;
using CatalogService.Application.Stations.Queries.GetStations;
using CatalogService.Domain.Entities;
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
            ThumbnailImageUrl = thumbnailImageUrl
        };

        await repo.AddAsync(station, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return id;
    }

    public async Task<Guid> UpdateAsync(UpdateStationCommand command, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.GetRepository<Station, Guid>();

        var route = await repo.GetByIdAsync(command.Id, cancellationToken);
        if (route == null)
        {
            return Guid.Empty;
        }

        if (command.ThumbnailImageStream != null && command.ThumbnailImageFileName != null)
        {
            var blobName = route.Id + GetFileType(command.ThumbnailImageFileName);
            var containerName = _configuration["Azure:BlobStorageSettings:StationImagesContainerName"] ?? "station-images";
            var blobUrl = await _azureBlobService.UploadAsync(
                command.ThumbnailImageStream,
                blobName,
                containerName);
            route.ThumbnailImageUrl = blobUrl;
        }

        route.Name = command.Name;
        route.StreetNumber = command.StreetNumber;
        route.Street = command.Street;
        route.Ward = command.Ward;
        route.District = command.District;
        route.City = command.City;

        await repo.UpdateAsync(route, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return route.Id;
    }

    public async Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.GetRepository<Station, Guid>();

        var route = await repo.GetByIdAsync(id, cancellationToken);
        if (route == null)
        {
            return Guid.Empty;
        }

        route.DeleteFlag = true;

        await repo.UpdateAsync(route, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return route.Id;
    }

    public async Task<(IEnumerable<StationsResponseDto>, int)> GetAsync(
        GetStationsQuery query,
        int sizePerPage,
        CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.GetRepository<Station, Guid>();

        Expression<Func<Station, bool>> filter = GetFilter(query);

        var stations = await repo.GetPagedAsync(
            skip: query.Page * sizePerPage,
            take: sizePerPage,
            filters: [filter],
            cancellationToken: cancellationToken);

        var totalPages = await repo.GetTotalPagesAsync(sizePerPage, [filter], cancellationToken);

        return (
            stations.Select(s => new StationsResponseDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                StreetNumber = s.StreetNumber,
                Street = s.Street,
                Ward = s.Ward,
                District = s.District,
                City = s.City,
                ThumbnailImageUrl = s.ThumbnailImageUrl
            }), totalPages);

    }

    public async Task<StationsResponseDto?> GetByIdAsync(Guid queryId, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Station, Guid>();

        return await repo.GetByIdAsync(queryId, cancellationToken)
            .ContinueWith(task =>
            {
                var station = task.Result;
                if (station == null) return null;

                return new StationsResponseDto
                {
                    Id = station.Id,
                    Code = station.Code,
                    Name = station.Name,
                    StreetNumber = station.StreetNumber,
                    Street = station.Street,
                    Ward = station.Ward,
                    District = station.District,
                    City = station.City,
                    ThumbnailImageUrl = station.ThumbnailImageUrl
                };
            }, cancellationToken);
    }

    #region Helper method

    private Expression<Func<Station, bool>> GetFilter(GetStationsQuery query)
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
            stationList = await repo.Query()
                .Where(s => s.DeleteFlag == false).ToListAsync(cancellationToken);

            response.Stations = stationList.Select(s => s.togGetNameStationResponseDto()).ToList();

            return response;
        }
        var normalizedSearch = searchString.Trim().RemoveDiacritics();


        var rawStations = await repo.Query()
            .Where(s => s.DeleteFlag == false && s.Name != null)
            .ToListAsync(cancellationToken);


        stationList = rawStations
        .Where(s => compareStationName(normalizedSearch, s.Name))
        .ToList();

        response.Stations = stationList.Select(s => s.togGetNameStationResponseDto()).ToList();

        return response;
    }
}
