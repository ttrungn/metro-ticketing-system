using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Buses.Commands.CreateBus;

public record CreateBusCommand : IRequest<ServiceResponse<Guid>>
{
    public Guid StationId { get; init; }
    public string? DestinationName { get; init; }
}

public class CreateBusCommandValidator : AbstractValidator<CreateBusCommand>
{
    public CreateBusCommandValidator()
    {
        RuleFor(x => x.StationId)
            .NotEmpty().WithMessage("Xin vui lòng nhập ID trạm!");

        RuleFor(x => x.DestinationName)
            .NotEmpty().WithMessage("Xin vui lòng nhập điểm đến!")
            .MaximumLength(256).WithMessage("Điểm đến không được vượt quá 256 ký tự!");
    }
}

public class CreateBusCommandHandler : IRequestHandler<CreateBusCommand, ServiceResponse<Guid>>
{
    private readonly IBusService _busService;
    private readonly ILogger<CreateBusCommandHandler> _logger;

    public CreateBusCommandHandler(IBusService busService, ILogger<CreateBusCommandHandler> logger)
    {
        _busService = busService;
        _logger = logger;
    }

    public async Task<ServiceResponse<Guid>> Handle(CreateBusCommand command, CancellationToken cancellationToken)
    {
        var busId = await _busService.CreateAsync(command, cancellationToken);

        if (busId == Guid.Empty)
        {
            _logger.LogWarning("Station with ID {StationId} not found.", command.StationId);
            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Không tìm thấy trạm!",
                Data = Guid.Empty
            };
        }

        _logger.LogInformation("Bus created with ID: {busId}", busId);
        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Tạo bus thành công!",
            Data = busId
        };
    }
}
