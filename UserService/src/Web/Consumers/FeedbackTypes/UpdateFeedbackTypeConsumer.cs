using BuildingBlocks.Domain.Events.FeedbackTypes;
using MassTransit;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Feedbacks.DTOs;

namespace UserService.Web.Consumers.FeedbackTypes;

public class UpdateFeedbackTypeConsumer : IConsumer<UpdateFeedbackTypeEvent>
{
    private readonly ILogger<UpdateFeedbackTypeConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFeedbackTypeConsumer(ILogger<UpdateFeedbackTypeConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<UpdateFeedbackTypeEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("UpdateFeedbackTypeConsumer Message Received: {FeedbackTypeId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var feedbackTypeReadModel = await session.LoadAsync<FeedbackTypeReadModel>(message.Id);
        if (feedbackTypeReadModel != null)
        {
            feedbackTypeReadModel.Name = message.Name;
            feedbackTypeReadModel.Description = message.Description;
            feedbackTypeReadModel.LastModifiedAt = message.LastModifiedAt;

            session.Update(feedbackTypeReadModel);
            await session.SaveChangesAsync();
            _logger.LogInformation("FeedbackTypeReadModel Updated: {FeedbackTypeId}", feedbackTypeReadModel.Id);
            return;
        }

        _logger.LogWarning("FeedbackTypeReadModel not found for Id: {FeedbackTypeId}", message.Id);
    }
}
