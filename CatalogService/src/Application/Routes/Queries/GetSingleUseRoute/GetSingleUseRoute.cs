using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Routes.DTOs;
using CatalogService.Application.Tickets.DTO;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Routes.Queries.GetSingleUseRoute;

public record GetSingleUseRouteQuery : IRequest<ServiceResponse<IEnumerable<SingleUseGetRouteResponseDto>>>
{

}

public class GetSingleUseRouteQueryValidator : AbstractValidator<GetSingleUseRouteQuery>
{
    public GetSingleUseRouteQueryValidator()
    {
    }
}

public class GetSingleUseRouteQueryHandler : IRequestHandler<GetSingleUseRouteQuery, ServiceResponse<IEnumerable<SingleUseGetRouteResponseDto>>>
{
    private readonly IRouteService _service;
    private readonly ILogger<GetSingleUseRouteQueryHandler> _logger;

    public GetSingleUseRouteQueryHandler(IRouteService service, ILogger<GetSingleUseRouteQueryHandler>  logger)
    {
        _logger = logger;
        _service = service;
    }

    public async Task<ServiceResponse<IEnumerable<SingleUseGetRouteResponseDto>>> Handle(GetSingleUseRouteQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetSingleUseRouteQueryHandler");

        var response = await _service.GetSingleUseRoutesAsync();

        _logger.LogInformation("Route List ${response}", response);

        return new ServiceResponse<IEnumerable<SingleUseGetRouteResponseDto>>{
            Data = response,
            Message = "Get Service Response Successfully",
            Succeeded = true
        };

    }
}
