using BuildingBlocks.Domain.Events.Users;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Application.Users.EventHandlers;

public class CreateStudentRequestEventHandler : INotificationHandler<CreateStudentRequestEvent>
{
    private readonly ILogger<CreateStudentRequestEventHandler> _logger;
    private readonly IMassTransitService<CreateStudentRequestEvent> _massTransitService;
    
    public CreateStudentRequestEventHandler(ILogger<CreateStudentRequestEventHandler> logger, IMassTransitService<CreateStudentRequestEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }
    public async Task Handle(CreateStudentRequestEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending CreateStudentRequestEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("CreateStudentRequestEvent sent to Kafka");
    }
}
