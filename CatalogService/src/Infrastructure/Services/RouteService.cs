using System.Linq;
using System.Linq.Expressions;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Routes.Commands.CreateRoute;
using CatalogService.Application.Routes.Commands.UpdateRoute;
using CatalogService.Application.Routes.Commands.UpsertRouteStation;
using CatalogService.Application.Routes.DTOs;
using CatalogService.Application.Routes.Queries.GetRoutes;
using CatalogService.Application.Tickets.DTO;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CatalogService.Infrastructure.Services;

public class RouteService : IRouteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAzureBlobService _azureBlobService;
    private readonly IConfiguration _configuration;

    public RouteService(IUnitOfWork unitOfWork, IAzureBlobService azureBlobService,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _azureBlobService = azureBlobService;
        _configuration = configuration;
    }

    public async Task<Guid> CreateAsync(
        CreateRouteCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();

        var repo = _unitOfWork.GetRepository<Route, Guid>();

        var count = repo.Query().Count();
        var code = GenerateCode(count);

        var thumbnailImageUrl = "empty";
        if (command.ThumbnailImageStream != null && command.ThumbnailImageFileName != null)
        {
            var blobName = id + GetFileType(command.ThumbnailImageFileName);
            var containerName =
                _configuration["Azure:BlobStorageSettings:RouteImagesContainerName"] ??
                "route-images";
            var blobUrl = await _azureBlobService.UploadAsync(
                command.ThumbnailImageStream,
                blobName,
                containerName);
            thumbnailImageUrl = blobUrl;
        }

        var newRoute = new Route()
        {
            Id = id,
            Code = code,
            Name = command.Name,
            ThumbnailImageUrl = thumbnailImageUrl,
            LengthInKm = command.LengthInKm,
        };

        await repo.AddAsync(newRoute, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return id;
    }

    public async Task<Guid> UpdateAsync(UpdateRouteCommand command,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Route, Guid>();

        var route = await repo.GetByIdAsync(command.Id, cancellationToken);
        if (route == null)
        {
            return Guid.Empty;
        }

        if (command.ThumbnailImageStream != null && command.ThumbnailImageFileName != null)
        {
            var blobName = route.Id + GetFileType(command.ThumbnailImageFileName);
            var containerName =
                _configuration["Azure:BlobStorageSettings:RouteImagesContainerName"] ??
                "route-images";
            var blobUrl = await _azureBlobService.UploadAsync(
                command.ThumbnailImageStream,
                blobName,
                containerName);
            route.ThumbnailImageUrl = blobUrl;
        }

        route.Name = command.Name;
        if (command.LengthInKm > 0.1)
        {
            route.LengthInKm = (double)command.LengthInKm!;
        }

        await repo.UpdateAsync(route, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return route.Id;
    }

    public async Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Route, Guid>();

        var route = await repo.GetByIdAsync(id, cancellationToken);

        if (route == null)
        {
            return Guid.Empty;
        }

        var stationRouteRepo =
            _unitOfWork.GetRepository<StationRoute, (Guid StationId, Guid RouteId)>();

        var stationRoutes = await stationRouteRepo.Query().Where(r => r.RouteId == id).ToListAsync(cancellationToken);
        if (stationRoutes.Count != 0)
        {
            foreach (var stationRoute in stationRoutes)
            {
                stationRoute.DeleteFlag = true;
                await stationRouteRepo.UpdateAsync(stationRoute, cancellationToken);
            }
        }

        route.DeleteFlag = true;

        await repo.UpdateAsync(route, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return route.Id;
    }

    public async Task<(IEnumerable<RoutesResponseDto>, int)> GetAsync(
        GetRoutesQuery query,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Route, Guid>();

        Expression<Func<Route, bool>> filter = GetFilter(query);

        var routes = await repo.GetPagedAsync(
            skip: query.Page * query.PageSize,
            take: query.PageSize,
            filters: [filter],
            cancellationToken: cancellationToken);

        var totalPages = await repo.GetTotalPagesAsync(query.PageSize, [filter], cancellationToken);

        return (
            routes.Select(r => new RoutesResponseDto
            {
                Id = r.Id,
                Code = r.Code,
                Name = r.Name,
                ThumbnailImageUrl = r.ThumbnailImageUrl,
                LengthInKm = r.LengthInKm
            }), totalPages);
    }

    public async Task<StationRouteResponseDto?> GetByIdAsync(Guid requestId,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Route, Guid>();

        var route = await repo.Query().Include(r => r.StationRoutes.Where(sr => sr.DeleteFlag == false)).ThenInclude(r => r.Station)
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);
        if (route == null)
        {
            return null;
        }

        var stationRoutes = route.StationRoutes.Select(sr => new StationResponseDto()
        {
            Id = sr.StationId,
            Name = sr.Station?.Name,
            Code = sr.Station?.Code,
            StreetNumber = sr.Station?.StreetNumber,
            Street = sr.Station?.Street,
            Ward = sr.Station?.Ward,
            District = sr.Station?.District,
            City = sr.Station?.City,
            ThumbnailImageUrl = sr.Station?.ThumbnailImageUrl,
            Order = sr.Order,
            DistanceToNext = sr.DistanceToNext,
        }).ToList();

        var response = new StationRouteResponseDto()
        {
            Id = route.Id,
            Name = route.Name,
            Code = route.Code,
            LengthInKm = route.LengthInKm,
            ThumbnailImageUrl = route.ThumbnailImageUrl,
            Stations = stationRoutes
        };

        return response;
    }


    public async Task<Guid> UpsertRouteStationAsync(UpsertStationRouteCommand command, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Route, Guid>();

        var stationRouteRepo = _unitOfWork.GetRepository<StationRoute, (Guid, Guid)>();

        var route = await repo.Query().Include(r => r.StationRoutes).FirstOrDefaultAsync(r => r.Id == command.Id);

        if (route == null)
            return Guid.Empty;

        double routeLength = 0;

        var existingList = route.StationRoutes.Where(sr => sr.DeleteFlag == false);

        foreach (var stationRouteDto in command.StationRoutes)
        {
            //If existing station route is found in both existingDict and command, update it

            
            var stationRoute = route.StationRoutes
        .FirstOrDefault(sr => sr.StationId == stationRouteDto.StationId);
            if (stationRoute != null)
            {
                routeLength += stationRouteDto.DistanceToNext;
                stationRoute.Order = stationRouteDto.Order;
                stationRoute.DistanceToNext = stationRouteDto.DistanceToNext;
                stationRoute.DeleteFlag = false; 
                await stationRouteRepo.UpdateAsync(stationRoute, cancellationToken);
            }
            else
            {
                routeLength += stationRouteDto.DistanceToNext;
                // Create new station route
                var newStationRoute = new StationRoute
                {
                    RouteId = command.Id,
                    StationId = stationRouteDto.StationId,
                    Order = stationRouteDto.Order,
                    DistanceToNext = stationRouteDto.DistanceToNext
                };
                await stationRouteRepo.AddAsync(newStationRoute, cancellationToken);
            }
        }
        foreach (var existingStationRoute in existingList)
        {
            if (existingStationRoute.DeleteFlag == false && !command.StationRoutes.Any(sr => sr.StationId == existingStationRoute.StationId))
            {
                // Delete station route if it is not in the command
                await stationRouteRepo.RemoveAsync(existingStationRoute, cancellationToken);
            }

        }
        // Update route length

        route.LengthInKm = routeLength;


        await repo.UpdateAsync(route, cancellationToken);


        await _unitOfWork.SaveChangesAsync();



        return command.Id;

    }

    #region Helper method

    private Expression<Func<Route, bool>> GetFilter(GetRoutesQuery query)
    {
        return (r) =>
            r.Name!.ToLower().Contains(query.Name!.ToLower() + "") &&
            r.DeleteFlag == query.Status;
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

    #endregion

    public async Task<IEnumerable<SingleUseGetRouteResponseDto>> GetSingleUseRoutesAsync(CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Route, Guid>();
        var routes = (await repo.GetAllAsync()).Where(r => r.DeleteFlag == false).ToList();
        var response = routes.Select(r => new SingleUseGetRouteResponseDto()
        {
            Id = r.Id,
            Name = r.Name,
        }).ToList();
        return response;
    }

}
