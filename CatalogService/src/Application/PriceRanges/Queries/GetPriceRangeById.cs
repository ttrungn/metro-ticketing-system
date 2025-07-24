using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.PriceRanges.DTOs;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.PriceRanges.Queries;

public record GetPriceRangeByIdQuery(Guid Id) : IRequest<ServiceResponse<PriceRangeDto>>;
public class GetPriceRangeByIdQueryValidator : AbstractValidator<GetPriceRangeByIdQuery>
{
    public GetPriceRangeByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Xin vui lòng điền ID của khoảng giá!");
    }
}
public class GetPriceRangeByIdQueryHandler : IRequestHandler<GetPriceRangeByIdQuery, ServiceResponse<PriceRangeDto>>
{
    private readonly IPriceRangeService _service;
    private readonly ILogger<GetPriceRangeByIdQueryHandler> _logger;

    public GetPriceRangeByIdQueryHandler(IPriceRangeService service, ILogger<GetPriceRangeByIdQueryHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<ServiceResponse<PriceRangeDto>> Handle(GetPriceRangeByIdQuery request, CancellationToken cancellationToken)
    {
        var priceRange = await _service.GetByIdAsync(request.Id, cancellationToken);
        if (priceRange == null)
        {
            _logger.LogWarning("No price range found with ID: {Id}", request.Id);
            return new ServiceResponse<PriceRangeDto>
            {
                Succeeded = false,
                Message = "Không tìm thấy khoảng giá với ID đã cho.",
                Data = null
            };
        }
        _logger.LogInformation("Retrieved price range with ID: {Id}", request.Id);
        return new ServiceResponse<PriceRangeDto>
        {
            Succeeded = true,
            Message = "Lấy khoảng giá thành công!",
            Data = priceRange
        };
    }
}
