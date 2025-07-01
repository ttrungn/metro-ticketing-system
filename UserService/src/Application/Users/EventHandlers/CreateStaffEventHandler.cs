using BuildingBlocks.Domain.Events.Users;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Application.Users.EventHandlers;

public class CreateStaffEventHandler : INotificationHandler<CreateStaffEvent>
{
    private readonly ILogger<CreateStaffEventHandler> _logger;
    private readonly IMassTransitService<CreateStaffEvent> _massTransitService;


    public CreateStaffEventHandler(ILogger<CreateStaffEventHandler> logger, IMassTransitService<CreateStaffEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }

    public async Task Handle(CreateStaffEvent createStaffEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing CreateStaffEvent to the message bus for Staff with Id: {StaffId}", createStaffEvent.StaffId);
        await _massTransitService.Produce(createStaffEvent, cancellationToken);
    }
}
