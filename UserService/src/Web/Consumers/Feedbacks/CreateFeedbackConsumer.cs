using BuildingBlocks.Domain.Events.Feedbacks;
using MassTransit;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Feedbacks.DTOs;

namespace UserService.Web.Consumers.Feedbacks;

public class CreateFeedbackConsumer : IConsumer<CreateFeedbackEvent>
{
    private readonly ILogger<CreateFeedbackConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateFeedbackConsumer(ILogger<CreateFeedbackConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<CreateFeedbackEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("CreateFeedbackConsumer Message Received: {FeedbackId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var feedbackReadModel = new FeedbackReadModel()
        {
            Id = message.Id,
            CustomerId = message.CustomerId,
            FeedbackTypeId = message.FeedbackTypeId,
            StationId = message.StationId,
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            LastModifiedAt = message.LastModifiedAt,
            DeletedAt = message.DeletedAt,
            DeleteFlag = message.DeleteFlag
        };

        session.Store(feedbackReadModel);
        await session.SaveChangesAsync();
        _logger.LogInformation("FeedbackReadModel Created: {FeedbackId}", feedbackReadModel.Id);
    }
}
