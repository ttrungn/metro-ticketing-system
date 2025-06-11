using System.Linq.Expressions;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Routes.Commands.CreateRoute;
using CatalogService.Application.Routes.Commands.UpdateRoute;
using CatalogService.Application.Routes.DTOs;
using CatalogService.Application.Routes.Queries.GetRoutes;
using CatalogService.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace CatalogService.Infrastructure.Services;

public class RouteService : IRouteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAzureBlobService _azureBlobService;
    private readonly IConfiguration _configuration;

    public RouteService(IUnitOfWork unitOfWork, IAzureBlobService azureBlobService, IConfiguration configuration)
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
            var containerName = _configuration["Azure:BlobStorageSettings:RouteImagesContainerName"] ?? "route-images";
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
            var containerName = _configuration["Azure:BlobStorageSettings:RouteImagesContainerName"] ?? "route-images";
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

        route.DeleteFlag = true;

        await repo.UpdateAsync(route, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return route.Id;
    }

    public async Task<(IEnumerable<RoutesResponseDto>, int)> GetAsync(
        GetRoutesQuery query,
        int sizePerPage,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Route, Guid>();

        Expression<Func<Route, bool>> filter = GetFilter(query);

        var routes = await repo.GetPagedAsync(
            skip: query.Page * sizePerPage,
            take: sizePerPage,
            filters: [filter],
            cancellationToken: cancellationToken);

        var totalPages = await repo.GetTotalPagesAsync(sizePerPage, [filter], cancellationToken);

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

    public async Task<RoutesResponseDto?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Route, Guid>();

        return await repo.GetByIdAsync(requestId, cancellationToken)
            .ContinueWith(task =>
            {
                var route = task.Result;
                if (route == null) return null;

                return new RoutesResponseDto
                {
                    Id = route.Id,
                    Code = route.Code,
                    Name = route.Name,
                    ThumbnailImageUrl = route.ThumbnailImageUrl,
                    LengthInKm = route.LengthInKm
                };
            }, cancellationToken);
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
}
