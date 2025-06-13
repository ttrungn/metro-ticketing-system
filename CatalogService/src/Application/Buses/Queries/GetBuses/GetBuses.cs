using BuildingBlocks.Response;
using CatalogService.Application.Buses.DTOs;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Buses.Queries.GetBuses;

public record GetBusesQuery : IRequest<ServiceResponse<GetBusesResponseDto>>
{
    public int Page { get; init; } = 0;
    public Guid? StationId { get; init; } = Guid.Empty;
    public string? DestinationName { get; init; } = string.Empty;
    public bool? Status { get; init; } = false;
}

public class GetBusesQueryHandler : IRequestHandler<GetBusesQuery, ServiceResponse<GetBusesResponseDto>>
{
    private readonly IBusService _busService;
    private readonly ILogger<GetBusesQueryHandler> _logger;

    private const int DefaultPageSize = 8;

    public GetBusesQueryHandler(ILogger<GetBusesQueryHandler> logger, IBusService busService)
    {
        _logger = logger;
        _busService = busService;
    }

    public async Task<ServiceResponse<GetBusesResponseDto>> Handle(GetBusesQuery request, CancellationToken cancellationToken)
    {
        var (buses, totalPages) = await _busService.GetAsync(request, DefaultPageSize, cancellationToken);

        var response = new GetBusesResponseDto
        {
            Buses = buses,
            TotalPages = totalPages,
            CurrentPage = request.Page,
        };

        return new ServiceResponse<GetBusesResponseDto>
        {
            Succeeded = true,
            Message = "Lấy danh sách bus thành công!",
            Data = response
        };
    }
}
