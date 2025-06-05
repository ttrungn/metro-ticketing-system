using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Routes.DTOs;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Routes.Queries.GetRoutes;

public record GetRoutesQuery : IRequest<ServiceResponse<GetRoutesResponseDto>>
{
    public int Page { get; set; } = 0;
    public string? Name { get; set; } = string.Empty;
}

public class GetRoutesQueryHandler : IRequestHandler<GetRoutesQuery, ServiceResponse<GetRoutesResponseDto>>
{

    private readonly IRouteService _routeService;
    private readonly ILogger<GetRoutesQueryHandler> _logger;

    private const int DefaultPageSize = 8;

    public GetRoutesQueryHandler(IRouteService routeService, ILogger<GetRoutesQueryHandler> logger)
    {
        _routeService = routeService;
        _logger = logger;
    }

    public async Task<ServiceResponse<GetRoutesResponseDto>> Handle(GetRoutesQuery request, CancellationToken cancellationToken)
    {
        var (routes, totalPages) = await _routeService.GetAsync(request, DefaultPageSize, cancellationToken);

        var response = new GetRoutesResponseDto
        {
            Routes = routes,
            TotalPages = totalPages,
            CurrentPage = request.Page,
        };

        return new ServiceResponse<GetRoutesResponseDto>()
        {
            Succeeded = true,
            Message = "Lấy danh sách tuyến thành công!",
            Data = response
        };
    }
}
