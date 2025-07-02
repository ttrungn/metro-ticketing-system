using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Stations.DTOs;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Stations.Queries.GetStationById;

public record GetStationByIdQuery(Guid Id) : IRequest<ServiceResponse<StationReadModel>>;

public class GetStationByIdQueryValidator : AbstractValidator<GetStationByIdQuery>
{
    public GetStationByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Xin vui lòng nhập ID trạm!");
        RuleFor(x => x.Id)
            .Must(id => id != Guid.Empty).WithMessage("ID trạm không được là Guid.Empty!");
    }
}

public class GetStationByIdQueryHandler : IRequestHandler<GetStationByIdQuery, ServiceResponse<StationReadModel>>
{
    private readonly IStationService _stationService;
    private readonly ILogger<GetStationByIdQueryHandler> _logger;

    public GetStationByIdQueryHandler(IStationService stationService, ILogger<GetStationByIdQueryHandler> logger)
    {
        _stationService = stationService;
        _logger = logger;
    }

    public async Task<ServiceResponse<StationReadModel>> Handle(GetStationByIdQuery query, CancellationToken cancellationToken)
    {
        var station = await _stationService.GetByIdAsync(query.Id, cancellationToken);

        if (station == null)
        {
            _logger.LogWarning("Station with ID {RouteId} not found.", query.Id);
            return new ServiceResponse<StationReadModel>
            {
                Succeeded = false,
                Message = "Không tìm thấy trạm!",
                Data = null
            };
        }

        _logger.LogInformation("Station with ID {RouteId} retrieved successfully.", query.Id);

        return new ServiceResponse<StationReadModel>
        {
            Succeeded = true,
            Message = "Lấy thông tin trạm thành công!",
            Data = station
        };
    }
}
