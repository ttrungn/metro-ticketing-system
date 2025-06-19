using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Buses.Commands.UpdateBus;

public record UpdateBusCommand : IRequest<ServiceResponse<Guid>>
{
    public Guid Id { get; init; }
    public Guid? StationId { get; init; }
    public string DestinationName { get; init; } = null!;
}

public class UpdateBusCommandValidator : AbstractValidator<UpdateBusCommand>
{
    public UpdateBusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Xin vui lòng nhập ID bus!");

        RuleFor(x => x.DestinationName)
            .MaximumLength(256).WithMessage("Điểm đến không được vượt quá 256 ký tự!");
    }
}

public class UpdateBusCommandHandler : IRequestHandler<UpdateBusCommand, ServiceResponse<Guid>>
{
    private readonly IBusService _busService;
    private readonly ILogger<UpdateBusCommandHandler> _logger;

    public UpdateBusCommandHandler(IBusService busService, ILogger<UpdateBusCommandHandler> logger)
    {
        _busService = busService;
        _logger = logger;
    }

    public async Task<ServiceResponse<Guid>> Handle(UpdateBusCommand command, CancellationToken cancellationToken)
    {
        var busId = await _busService.UpdateAsync(command, cancellationToken);

        if (busId == Guid.Empty)
        {
            _logger.LogWarning("Bus ID {BusId} or Station ID {stationId} not found.", command.Id, command.StationId);
            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Không tìm thấy bus hoặc trạm!",
                Data = Guid.Empty
            };
        }

        _logger.LogInformation("Bus updated with ID: {busId}", busId);
        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Cập nhật bus thành công!",
            Data = busId
        };
    }
}
