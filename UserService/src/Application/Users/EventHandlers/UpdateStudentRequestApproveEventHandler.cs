using BuildingBlocks.Domain.Events.Users;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Application.Users.EventHandlers;

public class UpdateStudentRequestApproveEventHandler : INotificationHandler<UpdateStudentRequestApproveEvent>
{
    private readonly ILogger<UpdateStudentRequestApproveEventHandler> _logger;
    private readonly IMassTransitService<UpdateStudentRequestApproveEvent> _massTransitService;
    
    public UpdateStudentRequestApproveEventHandler(ILogger<UpdateStudentRequestApproveEventHandler> logger, 
        IMassTransitService<UpdateStudentRequestApproveEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }
    public async Task Handle(UpdateStudentRequestApproveEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending UpdateStudentRequestApproveEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("UpdateStudentRequestApproveEvent sent to Kafka");
    }
}
