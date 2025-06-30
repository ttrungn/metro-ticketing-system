using BuildingBlocks.Response;
using CatalogService.Application.Buses.DTOs;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Buses.Queries.GetBusById;

public record GetBusByIdQuery(Guid Id) : IRequest<ServiceResponse<BusReadModel>>;

public class GetBusByIdQueryValidator : AbstractValidator<GetBusByIdQuery>
{
    public GetBusByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Xin vui lòng nhập ID của bus!");
    }
}

public class GetBusByIdQueryHandler : IRequestHandler<GetBusByIdQuery, ServiceResponse<BusReadModel>>
{
    private readonly IBusService _busService;
    private readonly ILogger<GetBusByIdQueryHandler> _logger;
    public GetBusByIdQueryHandler(ILogger<GetBusByIdQueryHandler> logger, IBusService busService)
    {
        _logger = logger;
        _busService = busService;
    }

    public async Task<ServiceResponse<BusReadModel>> Handle(GetBusByIdQuery query, CancellationToken cancellationToken)
    {
        var bus = await _busService.GetByIdAsync(query.Id, cancellationToken);

        if (bus == null)
        {
            _logger.LogWarning("Bus with ID {BusId} not found.", query.Id);
            return new ServiceResponse<BusReadModel>()
            {
                Succeeded = false,
                Message = "Không tìm thấy bus!",
                Data = null
            };
        }

        _logger.LogInformation("Bus with ID {BusId} retrieved successfully", bus.Id);
        return new ServiceResponse<BusReadModel>()
        {
            Succeeded = true,
            Message = "Lấy thông tin bus thành công!",
            Data = bus
        };
    }
}
