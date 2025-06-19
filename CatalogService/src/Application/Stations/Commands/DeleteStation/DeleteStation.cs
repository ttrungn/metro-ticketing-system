using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Stations.Commands.DeleteStation;

public record DeleteStationCommand(Guid Id) : IRequest<ServiceResponse<Guid>>;


public class DeleteStationCommandValidator : AbstractValidator<DeleteStationCommand>
{
    public DeleteStationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Xin vui lòng nhập ID của trạm!");
    }
}

public class DeleteStationCommandHandler : IRequestHandler<DeleteStationCommand, ServiceResponse<Guid>>
{
    private readonly IStationService _stationService;
    private readonly ILogger<DeleteStationCommandHandler> _logger;


    public DeleteStationCommandHandler(IStationService stationService, ILogger<DeleteStationCommandHandler> logger)
    {
        _stationService = stationService;
        _logger = logger;
    }

    public async Task<ServiceResponse<Guid>> Handle(DeleteStationCommand command, CancellationToken cancellationToken)
    {
        var stationId = await _stationService.DeleteAsync(command.Id, cancellationToken);

        if (stationId == Guid.Empty)
        {
            _logger.LogWarning("Station with ID {StationId} not found for deletion.", command.Id);

            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Không tìm thấy trạm!",
                Data = Guid.Empty
            };
        }

        _logger.LogInformation("Delete updated with ID: {RouteId}", stationId);

        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Xóa trạm thành công!",
            Data = stationId
        };
    }
}
