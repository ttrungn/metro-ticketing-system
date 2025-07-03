using BuildingBlocks.Domain.Events.FeedbackTypes;
using MassTransit;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Feedbacks.DTOs;

namespace UserService.Web.Consumers.FeedbackTypes;

public class DeleteFeedbackTypeConsumer : IConsumer<DeleteFeedbackTypeEvent>
{
    private readonly ILogger<DeleteFeedbackTypeConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteFeedbackTypeConsumer(ILogger<DeleteFeedbackTypeConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<DeleteFeedbackTypeEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("DeleteFeedbackTypeConsumer Message Received: {FeedbackTypeId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var feedbackTypeReadModel = await session.LoadAsync<FeedbackTypeReadModel>(message.Id);
        if (feedbackTypeReadModel != null)
        {
            feedbackTypeReadModel.LastModifiedAt = message.LastModifiedAt;
            feedbackTypeReadModel.DeletedAt = message.DeletedAt;
            feedbackTypeReadModel.DeleteFlag = message.DeleteFlag;

            session.Update(feedbackTypeReadModel);
            await session.SaveChangesAsync();
            _logger.LogInformation("FeedbackTypeReadModel Deleted: {FeedbackTypeId}", feedbackTypeReadModel.Id);
            return;
        }
        _logger.LogInformation("FeedbackTypeReadModel not found for Id: {FeedbackTypeId}", message.Id);
    }
}
