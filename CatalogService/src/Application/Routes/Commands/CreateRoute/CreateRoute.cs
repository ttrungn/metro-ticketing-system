using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Routes.Commands.CreateRoute;

public record CreateRouteCommand : IRequest<ServiceResponse<Guid>>
{
    public string Code { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? ThumbnailImageUrl { get; init; }
    public double LengthInKm { get; init; }
}

public class CreateRouteCommandValidator : AbstractValidator<CreateRouteCommand>
{
    public CreateRouteCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Xin vui lòng nhập code.")
            .MinimumLength(6).WithMessage("Code yêu cầu 6 chữ số.")
            .MaximumLength(6).WithMessage("Code yêu cầu 6 chữ số.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Xin vui lòng nhập tên tuyến.");

        RuleFor(x => x.ThumbnailImageUrl)
            .MaximumLength(200).WithMessage("Đường dẫn ảnh không được vượt quá 200 ký tự.");

        RuleFor(x => x.LengthInKm)
            .GreaterThan(0).WithMessage("Chiều dài tuyến phải lớn hơn 0.");
    }
}

public class CreateRouteCommandHandler : IRequestHandler<CreateRouteCommand, ServiceResponse<Guid>>
{
    private readonly IRouteService _routeService;
    private readonly ILogger<CreateRouteCommandHandler> _logger;

    public CreateRouteCommandHandler(IRouteService routeService, ILogger<CreateRouteCommandHandler> logger)
    {
        _routeService = routeService;
        _logger = logger;
    }

    public async Task<ServiceResponse<Guid>> Handle(CreateRouteCommand request, CancellationToken cancellationToken)
    {
        var routeId = await _routeService.CreateAsync(request, cancellationToken);

        _logger.LogInformation("Route created with ID: {RouteId}", routeId);

        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Tạo tuyến thành công!",
            Data = routeId
        };
    }
}
