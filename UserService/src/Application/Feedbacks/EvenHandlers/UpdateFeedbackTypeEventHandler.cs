using BuildingBlocks.Domain.Events.FeedbackTypes;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Application.Feedbacks.EvenHandlers;

public class UpdateFeedbackTypeEventHandler : INotificationHandler<UpdateFeedbackTypeEvent>
{
    private readonly ILogger<UpdateFeedbackTypeEventHandler> _logger;
    private readonly IMassTransitService<UpdateFeedbackTypeEvent> _massTransitService;

    public UpdateFeedbackTypeEventHandler(IMassTransitService<UpdateFeedbackTypeEvent> massTransitService, ILogger<UpdateFeedbackTypeEventHandler> logger)
    {
        _massTransitService = massTransitService;
        _logger = logger;
    }

    public async Task Handle(UpdateFeedbackTypeEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending UpdateFeedbackTypeEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("UpdateFeedbackTypeEvent sent to Kafka");
    }
}
