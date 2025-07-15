using System.Text.Json.Serialization;
using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Routes.Commands.UpdateRoute;

public record UpdateRouteCommand : IRequest<ServiceResponse<Guid>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public double? LengthInKm { get; init; }
    public Stream? ThumbnailImageStream { get; init; }
    public string? ThumbnailImageFileName { get; init; }
}

public class UpdateRouteCommandValidator : AbstractValidator<UpdateRouteCommand>
{
    public UpdateRouteCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Xin vui lòng nhập tên tuyến!");
    }
}

public class UpdateRouteCommandHandler : IRequestHandler<UpdateRouteCommand, ServiceResponse<Guid>>
{
    private readonly IRouteService _routeService;
    private readonly ILogger<UpdateRouteCommandHandler> _logger;

    public UpdateRouteCommandHandler(IRouteService routeService, ILogger<UpdateRouteCommandHandler> logger)
    {
        _routeService = routeService;
        _logger = logger;
    }

    public async Task<ServiceResponse<Guid>> Handle(UpdateRouteCommand request, CancellationToken cancellationToken)
    {
        var routeId = await _routeService.UpdateAsync(request, cancellationToken);
        if (routeId == Guid.Empty)
        {
            _logger.LogWarning("Route with ID {RouteId} not found.", request.Id);
            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Không tìm thấy tuyến!",
                Data = Guid.Empty
            };
        }

        _logger.LogInformation("Route updated with ID: {RouteId}", routeId);
        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Chỉnh sửa tuyến thành công!",
            Data = routeId
        };
    }
}
