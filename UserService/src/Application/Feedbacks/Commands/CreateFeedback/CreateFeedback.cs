using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Application.Feedbacks.Commands.CreateFeedback;

public record CreateFeedbackCommand : IRequest<ServiceResponse<Guid>>
{
    public Guid FeedbackTypeId { get; init; } = Guid.Empty;
    public string Content { get; init; } = null!;
    public string? StationId { get; init; } = null!;
}

public class CreateFeedbackCommandValidator : AbstractValidator<CreateFeedbackCommand>
{
    public CreateFeedbackCommandValidator()
    {
        RuleFor(x => x.FeedbackTypeId)
            .NotEmpty().WithMessage("Vui lòng chọn loại feedback!");
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Vui lòng nhập nội dung feedback!");
    }
}

public class CreateFeedbackCommandHandler : IRequestHandler<CreateFeedbackCommand, ServiceResponse<Guid>>
{
    private readonly IUser _user;
    private readonly IFeedbackService _feedbackService;
    private readonly ILogger<CreateFeedbackCommandHandler> _logger;

    public CreateFeedbackCommandHandler(IFeedbackService feedbackService, ILogger<CreateFeedbackCommandHandler> logger, IUser user)
    {
        _feedbackService = feedbackService;
        _logger = logger;
        _user = user;
    }

    public async Task<ServiceResponse<Guid>> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
    {
        var feedbackId = await _feedbackService.CreateAsync(request, _user.Id!, cancellationToken);

        if (feedbackId == Guid.Empty)
        {
            _logger.LogError("Failed to create feedback for user ID: {UserId}", _user.Id);
            return new ServiceResponse<Guid>
            {
                Succeeded = false,
                Message = "Tạo feedback thất bại!",
                Data = Guid.Empty
            };
        }

        _logger.LogInformation("Feedback created with ID: {FeedbackId}", feedbackId);
        return new ServiceResponse<Guid>
        {
            Succeeded = true,
            Message = "Tạo feedback thành công!",
            Data = feedbackId
        };

    }
}
