using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Routes.DTOs;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Routes.Queries.GetRoutes;

public record GetRoutesQuery : IRequest<ServiceResponse<GetRoutesResponseDto>>
{
    public int Page { get; init; } = 0;
    public int PageSize { get; init; } = 8;
    public string? Name { get; init; } = string.Empty;
    public bool? Status { get; init; } = false;
}

public class GetRoutesQueryValidator : AbstractValidator<GetRoutesQuery>
{
    public GetRoutesQueryValidator()
    {
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Kích thước trang phải lớn hơn 0!");
    }
}

public class GetRoutesQueryHandler : IRequestHandler<GetRoutesQuery, ServiceResponse<GetRoutesResponseDto>>
{

    private readonly IRouteService _routeService;
    private readonly ILogger<GetRoutesQueryHandler> _logger;

    public GetRoutesQueryHandler(IRouteService routeService, ILogger<GetRoutesQueryHandler> logger)
    {
        _routeService = routeService;
        _logger = logger;
    }

    public async Task<ServiceResponse<GetRoutesResponseDto>> Handle(GetRoutesQuery request, CancellationToken cancellationToken)
    {
        var (routes, totalPages) = await _routeService.GetAsync(request, cancellationToken);

        var response = new GetRoutesResponseDto
        {
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            Routes = routes,
        };

        _logger.LogInformation("Retrieve routes successfully: Total pages: {TotalPages} - Current page: {CurrentPage} - Page size: {PageSize}",
            response.TotalPages, response.CurrentPage, response.PageSize);
        return new ServiceResponse<GetRoutesResponseDto>()
        {
            Succeeded = true,
            Message = "Lấy danh sách tuyến thành công!",
            Data = response
        };
    }
}
