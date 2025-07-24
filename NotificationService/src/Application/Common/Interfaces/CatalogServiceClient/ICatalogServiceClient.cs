using BuildingBlocks.Domain.Common;
using BuildingBlocks.Response;
using CatalogService.Application.Tickets.DTO;

namespace NotificationService.Application.Common.Interfaces.CatalogServiceClient;

public interface ICatalogServiceClient
{
    Task<ServiceResponse<GetTicketsResponseDto>> GetTicketsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ServiceResponse<GetStationsResponseDto>> GetStationsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
