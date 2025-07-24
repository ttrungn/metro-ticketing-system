using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.PriceRanges.DTOs;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.PriceRanges.Queries;

public class GetPriceRangesQuery : IRequest<ServiceResponse<GetPriceRangeResponseDto>>
{
    public int Page { get; init; } = 0;
    public int PageSize { get; init; } = 8;
}
public class GetPriceRangesValidator : AbstractValidator<GetPriceRangesQuery>
{
    public GetPriceRangesValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(0).WithMessage("Trang phải lớn hơn hoặc bằng 0!");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Kích thước trang phải lớn hơn 0!");
    }
}
public class GetPriceRangesHandler : IRequestHandler<GetPriceRangesQuery, ServiceResponse<GetPriceRangeResponseDto>>
{
    private readonly IPriceRangeService _priceRangeService;
    private readonly ILogger<GetPriceRangesHandler> _logger;

    public GetPriceRangesHandler(IPriceRangeService priceRangeService, ILogger<GetPriceRangesHandler> logger)
    {
        _priceRangeService = priceRangeService;
        _logger = logger;
    }

    public async Task<ServiceResponse<GetPriceRangeResponseDto>> Handle(GetPriceRangesQuery request, CancellationToken cancellationToken)
    {
        var (priceRanges, totalPages) = await _priceRangeService.GetAsync(request, cancellationToken);

        var response = new GetPriceRangeResponseDto()
        {
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            PriceRanges = priceRanges,
        };
        _logger.LogInformation("Retrieved price ranges successfully: Total pages: {TotalPages} - Current page: {CurrentPage} - Page size: {PageSize}",
            totalPages, request.Page, request.PageSize);
        
        return new ServiceResponse<GetPriceRangeResponseDto>()
        {
            Succeeded = true,
            Message = "Lấy danh sách khoảng giá thành công!",
            Data = response
        };
    }
}
