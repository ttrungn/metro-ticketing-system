using BuildingBlocks.Response;
using CatalogService.Application.Buses.DTOs;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Buses.Queries.GetBuses;

public record GetBusesQuery : IRequest<ServiceResponse<GetBusesResponseDto>>
{
    public int Page { get; init; } = 0;
    public int PageSize { get; init; } = 8;
    public Guid? StationId { get; init; } = Guid.Empty;
    public string? DestinationName { get; init; } = string.Empty;
    public bool? Status { get; init; } = false;
}

public class GetBusesQueryValidator : AbstractValidator<GetBusesQuery>
{
    public GetBusesQueryValidator()
    {
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Kích thước trang phải lớn hơn 0!");
    }
}

public class GetBusesQueryHandler : IRequestHandler<GetBusesQuery, ServiceResponse<GetBusesResponseDto>>
{
    private readonly IBusService _busService;
    private readonly ILogger<GetBusesQueryHandler> _logger;

    public GetBusesQueryHandler(ILogger<GetBusesQueryHandler> logger, IBusService busService)
    {
        _logger = logger;
        _busService = busService;
    }

    public async Task<ServiceResponse<GetBusesResponseDto>> Handle(GetBusesQuery request, CancellationToken cancellationToken)
    {
        var (buses, totalPages) = await _busService.GetAsync(request, cancellationToken);

        var response = new GetBusesResponseDto
        {
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            Buses = buses,
        };
        _logger.LogInformation("Retrieve buses successfully: Total pages: {TotalPages} - Current page: {CurrentPage} - Page size: {PageSize}",
            response.TotalPages, response.CurrentPage, response.PageSize);
        return new ServiceResponse<GetBusesResponseDto>
        {
            Succeeded = true,
            Message = "Lấy danh sách bus thành công!",
            Data = response
        };
    }
}
