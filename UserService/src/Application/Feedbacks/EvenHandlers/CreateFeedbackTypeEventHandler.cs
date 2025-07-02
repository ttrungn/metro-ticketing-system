using BuildingBlocks.Domain.Events.FeedbackTypes;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Application.Feedbacks.EvenHandlers;

public class CreateFeedbackTypeEventHandler : INotificationHandler<CreateFeedbackTypeEvent>
{
    private readonly ILogger<CreateFeedbackTypeEventHandler> _logger;
    private readonly IMassTransitService<CreateFeedbackTypeEvent> _massTransitService;

    public CreateFeedbackTypeEventHandler(ILogger<CreateFeedbackTypeEventHandler> logger, IMassTransitService<CreateFeedbackTypeEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }

    public async Task Handle(CreateFeedbackTypeEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending CreateFeedbackTypeEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("CreateFeedbackTypeEvent sent to Kafka");
    }
}
