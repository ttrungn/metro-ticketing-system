using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Routes.DTOs;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Routes.Queries.GetRouteById;

public record GetRouteByIdQuery(Guid Id) : IRequest<ServiceResponse<StationRouteResponseDto>>;

public class GetRouteByIdQueryValidator : AbstractValidator<GetRouteByIdQuery>
{
    public GetRouteByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Xin vui lòng nhập ID tuyến!");
        RuleFor(x => x.Id)
            .Must(id => id != Guid.Empty).WithMessage("ID tuyến không được là Guid.Empty!");
    }
}

public class GetRouteByIdQueryHandler : IRequestHandler<GetRouteByIdQuery, ServiceResponse<StationRouteResponseDto>>
{
    private readonly IRouteService _routeService;
    private readonly ILogger<GetRouteByIdQueryHandler> _logger;

    public GetRouteByIdQueryHandler(IRouteService routeService, ILogger<GetRouteByIdQueryHandler> logger)
    {
        _routeService = routeService;
        _logger = logger;
    }

    public async Task<ServiceResponse<StationRouteResponseDto>> Handle(GetRouteByIdQuery request, CancellationToken cancellationToken)
    {
        var route = await _routeService.GetByIdAsync(request.Id, cancellationToken);

        if (route == null)
        {
            _logger.LogWarning("Route with ID {RouteId} not found", request.Id);
            return new ServiceResponse<StationRouteResponseDto>
            {
                Succeeded = false,
                Message = "Không tìm thấy tuyến!",
                Data = null
            };
        }

        _logger.LogInformation("Route with ID {RouteId} retrieved successfully", request.Id);

        return new ServiceResponse<StationRouteResponseDto>
        {
            Succeeded = true,
            Message = "Lấy thông tin tuyến thành công!",
            Data = route
        };
    }
}
