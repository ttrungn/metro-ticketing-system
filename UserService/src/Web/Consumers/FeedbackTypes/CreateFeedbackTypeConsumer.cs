using BuildingBlocks.Domain.Events.FeedbackTypes;
using MassTransit;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Feedbacks.DTOs;

namespace UserService.Web.Consumers.FeedbackTypes;

public class CreateFeedbackTypeConsumer : IConsumer<CreateFeedbackTypeEvent>
{
    private readonly ILogger<CreateFeedbackTypeConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateFeedbackTypeConsumer(ILogger<CreateFeedbackTypeConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<CreateFeedbackTypeEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("CreateFeedbackTypeConsumer Message Received: {FeedbackTypeId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();

        var feedbackTypeReadModel = new FeedbackTypeReadModel()
        {
            Id = message.Id,
            Name = message.Name,
            Description = message.Description,
            CreatedAt = message.CreatedAt,
            LastModifiedAt = message.LastModifiedAt,
            DeletedAt = message.DeletedAt,
            DeleteFlag = message.DeleteFlag
        };

        session.Store(feedbackTypeReadModel);
        await session.SaveChangesAsync();
        _logger.LogInformation("FeedbackTypeReadModel Created: {FeedbackTypeId}", feedbackTypeReadModel.Id);
    }
}
