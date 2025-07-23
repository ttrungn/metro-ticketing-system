using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.PriceRanges.Commands;

public record UpdatePriceRangeCommand : IRequest<ServiceResponse<Guid>>
{
    public Guid Id { get; init; }
    public int FromKm { get; init; }
    public int ToKm { get; init; }
    public decimal Price { get; init; }
}
public class UpdatePriceRangeValidator : AbstractValidator<UpdatePriceRangeCommand>
{
    public UpdatePriceRangeValidator()
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
public class UpdatePriceRangeCommandHandler : IRequestHandler<UpdatePriceRangeCommand, ServiceResponse<Guid>>
{
    private readonly IPriceRangeService _service;
    private readonly ILogger<UpdatePriceRangeCommandHandler> _logger;

    public UpdatePriceRangeCommandHandler(IPriceRangeService service, ILogger<UpdatePriceRangeCommandHandler> logger)
    {
        _service = service;
        _logger = logger;
    }
    
    public async Task<ServiceResponse<Guid>> Handle(UpdatePriceRangeCommand request, CancellationToken cancellationToken)
    {
        var priceRangeId = await _service.UpdateAsync(request, cancellationToken);
        if (priceRangeId == Guid.Empty)
        {
            _logger.LogWarning("Failed to update price range");
            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Cập nhật khoảng giá thất bại! Hãy kiểm tra lại khoảng giá.",
                Data = Guid.Empty
            };
        }
        _logger.LogInformation("Price range updated successfully with ID: {PriceRangeId}", priceRangeId);
        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Cập nhật khoảng giá thành công!",
            Data = priceRangeId
        };
    }
}
