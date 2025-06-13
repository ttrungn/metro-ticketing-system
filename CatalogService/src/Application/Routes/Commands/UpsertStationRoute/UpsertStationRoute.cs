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
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Route Id is required.");
        RuleFor(x => x.StationRoutes)
            .NotEmpty().WithMessage("At least one station route is required.")
            .Must(routes => routes.All(route => route.StationId != Guid.Empty && route.RouteId != Guid.Empty))
            .WithMessage("Each station route must have a valid StationId and RouteId.");
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



        //Check for duplicates in the station routes
        var duplicates = command.StationRoutes.GroupBy(sr => sr.StationId).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

        if (duplicates.Any())
        {
            _logger.LogWarning("Duplicate station routes found for stations: {Duplicates}", string.Join(", ", duplicates));
            return new ServiceResponse<Guid>
            {
                Succeeded = false,
                Message = "Duplicate station routes found.",
                Data = Guid.Empty,
            };
        }

        var sortedOrders = command.StationRoutes
            .Select(sr => sr.Order)
            .Distinct()
            .OrderBy(order => order)
            .ToList();
        bool isContiguous = sortedOrders.First() == 1 && sortedOrders.SequenceEqual(Enumerable.Range(1, sortedOrders.Count));

        if(!isContiguous)
        {
            _logger.LogWarning("Station route orders are not contiguous.");
            return new ServiceResponse<Guid>
            {
                Succeeded = false,
                Message = "Station route orders must be contiguous starting from 1.",
                Data = Guid.Empty,
            };
        }
        var sortedRoutes = command.StationRoutes
        .OrderBy(sr => sr.Order)
        .ToList();


        var lastStation = sortedRoutes.Last();
        if (lastStation.DistanceToNext != 0)
        {
            _logger.LogWarning("Last station must have distanceToNext equal to 0.");
            return new ServiceResponse<Guid>
            {
                Succeeded = false,
                Message = "Last station must have distanceToNext set to 0.",
                Data = Guid.Empty
            };
        }

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

