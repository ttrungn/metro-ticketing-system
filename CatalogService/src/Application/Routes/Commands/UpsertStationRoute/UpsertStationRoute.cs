using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Response;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Application.Routes.DTOs;
using CatalogService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Routes.Commands.UpsertRouteStation;

public record UpsertStationRouteCommand : IRequest<ServiceResponse<Guid>>
{
    public Guid Id { get; init; }    

    public IEnumerable<StationRouteDto> StationRoutes { get; init; } = new List<StationRouteDto>();

}


public class UpsertStationRouteCommandValidator : AbstractValidator<UpsertStationRouteCommand>
{
    public UpsertStationRouteCommandValidator()
    {
        //RuleFor(x => x.Code)
        //    .NotEmpty().WithMessage("Code is required.")
        //    .MaximumLength(50).WithMessage("Code must not exceed 50 characters.");
        //RuleFor(x => x.Name)
        //    .NotEmpty().WithMessage("Name is required.")
        //    .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        //RuleFor(x => x.ThumbnailImageUrl)
        //    .MaximumLength(200).WithMessage("ThumbnailImageUrl must not exceed 200 characters.");
        //RuleFor(x => x.LengthInKm)
        //    .GreaterThan(0).WithMessage("LengthInKm must be greater than zero.");
    }
}

public class UpdateStationRouteCommandHandler : IRequestHandler<UpsertStationRouteCommand, ServiceResponse<Guid>>
{
    private readonly ILogger<UpdateStationRouteCommandHandler> _logger;

    private readonly IRouteService _routeService;

    public UpdateStationRouteCommandHandler(ILogger<UpdateStationRouteCommandHandler> logger, IRouteService routeService)
    {
        _logger = logger;
        _routeService = routeService;
    }


    public async Task<ServiceResponse<Guid>> Handle(UpsertStationRouteCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Upsert route id " + command.Id);

        var routeId = await _routeService.UpsertRouteStationAsync(command, cancellationToken);

        if (routeId == Guid.Empty)
        {
            return new ServiceResponse<Guid>
            {
                Succeeded = false,
                Message = "Failed to upsert station route.",
                Data = Guid.Empty,
            };
        }

        return new ServiceResponse<Guid>
        {
            Succeeded = true,
            Message = "Upsert station route succesfully ",
            Data = routeId,
        };
    }
}

