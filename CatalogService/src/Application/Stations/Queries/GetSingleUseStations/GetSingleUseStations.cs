using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Routes.DTOs;
using CatalogService.Application.Tickets.DTO;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Stations.Queries.GetSingleUseStations;

public record GetSingleUseStationsQuery(Guid? routeId) : IRequest<ServiceResponse<IEnumerable<SingleUseGetStationsResponseDto>>>
{

}

public class GetSingleUseStationsQueryValidator : AbstractValidator<GetSingleUseStationsQuery>
{
    public GetSingleUseStationsQueryValidator()
    {
        RuleFor(x => x.routeId).NotEmpty().WithMessage("Route Id is required");
    }
}

public class GetSingleUseStationsQueryHandler : IRequestHandler<GetSingleUseStationsQuery, ServiceResponse<IEnumerable<SingleUseGetStationsResponseDto>>>
{
    private readonly IStationService _stationService;
    private readonly ILogger<GetSingleUseStationsQueryHandler> _logger;
    public GetSingleUseStationsQueryHandler(IStationService stationService, ILogger<GetSingleUseStationsQueryHandler> logger)
    {
        _stationService = stationService;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<SingleUseGetStationsResponseDto>>> Handle(GetSingleUseStationsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetSingleUseStationsQueryHandler");


        var response = await _stationService.GetStationsByRouteId(request.routeId ?? Guid.Empty, cancellationToken);

        if (response.Count() == 0)
        {
            return new ServiceResponse<IEnumerable<SingleUseGetStationsResponseDto>>
            {
                Data = response,
                Succeeded = true,
                Message = "No Stations Found"
            };
        }

        _logger.LogInformation("GetSingleUseStationsQueryHandler response: ${response}", response);
        return new ServiceResponse<IEnumerable<SingleUseGetStationsResponseDto>>
        {
            Data = response,
            Succeeded = true,
            Message = "Get Stations Successfully"
        };
    }
}
