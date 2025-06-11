using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Buses.Commands.DeleteBus;

public record DeleteBusCommand(Guid Id) : IRequest<ServiceResponse<Guid>>;

public class DeleteBusCommandValidator : AbstractValidator<DeleteBusCommand>
{
    public DeleteBusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Xin vui lòng nhập ID của bus!");
    }
}

public class DeleteBusCommandHandler : IRequestHandler<DeleteBusCommand, ServiceResponse<Guid>>
{

    private readonly IBusService _busService;
    private readonly ILogger<DeleteBusCommandHandler> _logger;

    public DeleteBusCommandHandler(ILogger<DeleteBusCommandHandler> logger, IBusService busService)
    {
        _logger = logger;
        _busService = busService;
    }

    public async Task<ServiceResponse<Guid>> Handle(DeleteBusCommand request, CancellationToken cancellationToken)
    {
        var busId = await _busService.DeleteAsync(request.Id, cancellationToken);

        if (busId == Guid.Empty)
        {
            _logger.LogWarning("Bus with ID {BusId} not found for deletion.", request.Id);
            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Không tìm thấy bus!",
                Data = Guid.Empty
            };
        }

        _logger.LogInformation("Bus deleted with ID: {BusId}", busId);
        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Xóa bus thành công!",
            Data = busId
        };
    }
}
