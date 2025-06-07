using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Stations.DTOs;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Stations.Queries.GetStations;

public record GetStationsQuery : IRequest<ServiceResponse<GetStationsResponseDto>>
{
    public int Page { get; init; } = 0;
    public string? Name { get; init; } = string.Empty;
    public bool? Status { get; init; } = false;
}

public class GetStationsQueryHandler : IRequestHandler<GetStationsQuery, ServiceResponse<GetStationsResponseDto>>
{
    private readonly IStationService _stationService;
    private readonly ILogger<GetStationsQueryHandler> _logger;

    private const int DefaultPageSize = 8;

    public GetStationsQueryHandler(ILogger<GetStationsQueryHandler> logger, IStationService stationService)
    {
        _logger = logger;
        _stationService = stationService;
    }

    public async Task<ServiceResponse<GetStationsResponseDto>> Handle(GetStationsQuery request, CancellationToken cancellationToken)
    {
        var (stations, totalPages) = await _stationService.GetAsync(request, DefaultPageSize, cancellationToken);

        var response = new GetStationsResponseDto
        {
            Stations = stations,
            TotalPages = totalPages,
            CurrentPage = request.Page,
        };

        return new ServiceResponse<GetStationsResponseDto>()
        {
            Succeeded = true,
            Message = "Lấy danh sách trạm thành công!",
            Data = response
        };
    }
}
