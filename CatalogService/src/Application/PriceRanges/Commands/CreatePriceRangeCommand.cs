using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.PriceRanges.Commands;

public record CreatePriceRangeCommand : IRequest<ServiceResponse<Guid>>
{
    public int FromKm { get; init; }
    public int ToKm { get; init; }
    public decimal Price { get; init; }
}
public class CreatePriceRangeValidator : AbstractValidator<CreatePriceRangeCommand>
{
    public CreatePriceRangeValidator()
    {
        RuleFor(x => x.FromKm)
            .GreaterThanOrEqualTo(0)
            .WithMessage("FromKm phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.ToKm)
            .GreaterThanOrEqualTo(0)
            .WithMessage("ToKm phải lớn hơn hoặc bằng 0.")
            .GreaterThan(x => x.FromKm)
            .WithMessage("ToKm phải lớn hơn FromKm.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price phải lớn hơn hoặc bằng 0.");
    }
}

public class CreatePriceRangeCommandHandler : IRequestHandler<CreatePriceRangeCommand, ServiceResponse<Guid>>
{
    private readonly IPriceRangeService _service;
    private readonly ILogger<CreatePriceRangeCommandHandler> _logger;
    public CreatePriceRangeCommandHandler(IPriceRangeService service, ILogger<CreatePriceRangeCommandHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<ServiceResponse<Guid>> Handle(CreatePriceRangeCommand request, CancellationToken cancellationToken)
    {
        var priceRangeId = await _service.CreateAsync(request, cancellationToken);
        if (priceRangeId == Guid.Empty)
        {
            _logger.LogWarning("Failed to create price range");
            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Tạo khoảng giá thất bại! Hãy kiểm tra lại khoảng giá.",
                Data = Guid.Empty
            };
        }
        _logger.LogInformation("Price range created successfully with ID: {PriceRangeId}", priceRangeId);
        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Tạo khoảng giá thành công!",
            Data = priceRangeId
        };
    }
}
