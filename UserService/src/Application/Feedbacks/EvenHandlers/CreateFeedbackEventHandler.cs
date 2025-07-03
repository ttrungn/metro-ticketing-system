using BuildingBlocks.Domain.Events.Feedbacks;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Application.Feedbacks.EvenHandlers;

public class CreateFeedbackEventHandler : INotificationHandler<CreateFeedbackEvent>
{
    private readonly ILogger<CreateFeedbackEventHandler> _logger;
    private readonly IMassTransitService<CreateFeedbackEvent> _massTransitService;

    public CreateFeedbackEventHandler(ILogger<CreateFeedbackEventHandler> logger, IMassTransitService<CreateFeedbackEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }

    public async Task Handle(CreateFeedbackEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending CreateFeedbackEvent to Kafka");
        await _massTransitService.Produce(notification, cancellationToken);
        _logger.LogInformation("CreateFeedbackEvent sent to Kafka");
    }
}
