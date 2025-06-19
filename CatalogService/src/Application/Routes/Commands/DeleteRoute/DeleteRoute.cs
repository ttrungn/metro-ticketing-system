using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Routes.Commands.DeleteRoute;

public record DeleteRouteCommand(Guid Id) : IRequest<ServiceResponse<Guid>>;


public class DeleteRouteCommandValidator : AbstractValidator<DeleteRouteCommand>
{
    public DeleteRouteCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Xin vui lòng nhập ID của tuyến.");
    }
}

public class DeleteRouteCommandHandler : IRequestHandler<DeleteRouteCommand, ServiceResponse<Guid>>
{
    private readonly ILogger<DeleteRouteCommandHandler> _logger;
    private readonly IRouteService _routeService;

    public DeleteRouteCommandHandler(ILogger<DeleteRouteCommandHandler> logger, IRouteService routeService)
    {
        _logger = logger;
        _routeService = routeService;
    }

    public async Task<ServiceResponse<Guid>>Handle(DeleteRouteCommand request, CancellationToken cancellationToken)
    {
        var routeId = await _routeService.DeleteAsync(request.Id, cancellationToken);

        if (routeId == Guid.Empty)
        {
            _logger.LogWarning("Route with ID {RouteId} not found for deletion.", request.Id);

            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Không tìm thấy tuyến!",
                Data = Guid.Empty
            };
        }

        _logger.LogInformation("Delete updated with ID: {RouteId}", routeId);

        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Xóa tuyến thành công!",
            Data = routeId
        };
    }
}
