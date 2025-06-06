using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Routes.DTOs;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Routes.Queries.GetRouteById;

public record GetRouteByIdQuery(Guid Id) : IRequest<ServiceResponse<RoutesResponseDto>>;

public class GetRouteByIdQueryValidator : AbstractValidator<GetRouteByIdQuery>
{
    public GetRouteByIdQueryValidator()
    {
    }
}

public class GetRouteByIdQueryHandler : IRequestHandler<GetRouteByIdQuery, ServiceResponse<RoutesResponseDto>>
{
    private readonly IRouteService _routeService;
    private readonly ILogger<GetRouteByIdQueryHandler> _logger;

    public GetRouteByIdQueryHandler(IRouteService routeService, ILogger<GetRouteByIdQueryHandler> logger)
    {
        _routeService = routeService;
        _logger = logger;
    }

    public async Task<ServiceResponse<RoutesResponseDto>> Handle(GetRouteByIdQuery request, CancellationToken cancellationToken)
    {
        var route = await _routeService.GetByIdAsync(request.Id, cancellationToken);

        if (route == null)
        {
            _logger.LogWarning("Route with ID {RouteId} not found", request.Id);
            return new ServiceResponse<RoutesResponseDto>
            {
                Succeeded = true,
                Message = "Tuyến không tồn tại"
            };
        }

        _logger.LogInformation("Route with ID {RouteId} retrieved successfully", request.Id);

        return new ServiceResponse<RoutesResponseDto>
        {
            Succeeded = true,
            Message = "Lấy thông tin tuyến thành công",
            Data = route
        };
    }
}
