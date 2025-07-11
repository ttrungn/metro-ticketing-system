using BuildingBlocks.Domain.Events.Users;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Application.Users.EventHandlers;

public class UpdateStudentRequestDeclinedEventHandler : INotificationHandler<UpdateStudentRequestDeclinedEvent>
{
    
    private readonly ILogger<UpdateStudentRequestDeclinedEventHandler> _logger;
    private readonly IMassTransitService<UpdateStudentRequestDeclinedEvent> _massTransitService;

    public UpdateStudentRequestDeclinedEventHandler(ILogger<UpdateStudentRequestDeclinedEventHandler> logger, IMassTransitService<UpdateStudentRequestDeclinedEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }

    public async Task Handle(UpdateStudentRequestDeclinedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending UpdateStudentRequestDeclinedEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("UpdateStudentRequestDeclinedEvent sent to Kafka");
    }
}
