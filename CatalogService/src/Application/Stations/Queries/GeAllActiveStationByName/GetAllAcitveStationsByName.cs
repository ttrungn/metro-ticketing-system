using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Stations.DTOs;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Stations.Queries.GeAllActiveStationByName;

public record GetAllAcitveStationsByNameQuery : IRequest<ServiceResponse<StationListResponseDto>>
{
    public string? Name { get; set; }
}

public class GetAllAcitveStationsByNameQueryValidator : AbstractValidator<GetAllAcitveStationsByNameQuery>
{
    public GetAllAcitveStationsByNameQueryValidator()
    {
    }
}

public class GetAllAcitveStationsByNameQueryHandler : IRequestHandler<GetAllAcitveStationsByNameQuery, ServiceResponse<StationListResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IStationService _service;
    private readonly ILogger<GetAllAcitveStationsByNameQueryHandler> _logger;

   public GetAllAcitveStationsByNameQueryHandler(IApplicationDbContext context,
       IStationService service, ILogger<GetAllAcitveStationsByNameQueryHandler> logger)
    {
        _context = context;
        _service = service;
        _logger = logger;
    }

    public async Task<ServiceResponse<StationListResponseDto>> Handle(GetAllAcitveStationsByNameQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetAllActiveStationsByName");
        
        if (request == null) {
            _logger.LogError("GetAllActiveStationByName error");
            return new ServiceResponse<StationListResponseDto>
            {
                Data = null,
                Message = "Failed to get Response",
                Succeeded = false
            };
        }

        var response = await _service.GetAllActiveStationsByName(request.Name?? "");

       if(response == null)
        {
            return new ServiceResponse<StationListResponseDto>
            {
                Data = null,
                Message = "Failed to get Response",
                Succeeded = false
            };
        }

        return new ServiceResponse<StationListResponseDto>
        {
            Data =  response,
            Message = "Get Stations Successfully",
            Succeeded = true,
        };
    }
}
