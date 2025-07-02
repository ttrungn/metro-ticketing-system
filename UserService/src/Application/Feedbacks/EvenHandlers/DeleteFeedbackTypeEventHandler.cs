using BuildingBlocks.Domain.Events.FeedbackTypes;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Application.Feedbacks.EvenHandlers;

public class DeleteFeedbackTypeEventHandler : INotificationHandler<DeleteFeedbackTypeEvent>
{
    private readonly ILogger<DeleteFeedbackTypeEventHandler> _logger;
    private readonly IMassTransitService<DeleteFeedbackTypeEvent> _massTransitService;

    public DeleteFeedbackTypeEventHandler(ILogger<DeleteFeedbackTypeEventHandler> logger, IMassTransitService<DeleteFeedbackTypeEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }

    public async Task Handle(DeleteFeedbackTypeEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending DeleteFeedbackTypeEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("DeleteFeedbackTypeEvent sent to Kafka");
    }
}
