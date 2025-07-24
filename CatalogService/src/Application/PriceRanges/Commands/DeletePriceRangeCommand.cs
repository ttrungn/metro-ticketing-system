using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.PriceRanges.Commands;

public record DeletePriceRangeCommand(Guid Id) : IRequest<ServiceResponse<Guid>>;

public class DeletePriceRangeCommandValidator : AbstractValidator<DeletePriceRangeCommand>
{
    public DeletePriceRangeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Xin vui lòng điền ID của khoảng giá!");
    }
}

public class DeletePriceRangeCommandHandler : IRequestHandler<DeletePriceRangeCommand, ServiceResponse<Guid>>
{
    private readonly IPriceRangeService _service;
    private readonly ILogger<DeletePriceRangeCommandHandler> _logger;

    public DeletePriceRangeCommandHandler(IPriceRangeService service, ILogger<DeletePriceRangeCommandHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<ServiceResponse<Guid>> Handle(DeletePriceRangeCommand request, CancellationToken cancellationToken)
    {
        var priceRangeId = await _service.DeleteAsync(request.Id, cancellationToken);
        if (priceRangeId == Guid.Empty)
        {
            _logger.LogWarning("Failed to delete price range with ID: {Id}", request.Id);
            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Xóa khoảng giá thất bại! Hãy kiểm tra lại khoảng giá.",
                Data = Guid.Empty
            };
        }
        _logger.LogInformation("Price range deleted successfully with ID: {Id}", priceRangeId);
        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Xóa khoảng giá thành công!",
            Data = priceRangeId
        };
    }
}
