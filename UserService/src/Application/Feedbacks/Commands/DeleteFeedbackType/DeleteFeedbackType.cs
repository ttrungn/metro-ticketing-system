using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Application.Feedbacks.Commands.DeleteFeedbackType;

public record DeleteFeedbackTypeCommand(Guid Id) : IRequest<ServiceResponse<Guid>>;

public class DeleteFeedbackTypeCommandValidator : AbstractValidator<DeleteFeedbackTypeCommand>
{
    public DeleteFeedbackTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Xin vui lòng nhập ID của loại phản hồi!");
    }
}

public class DeleteFeedbackTypeCommandHandler : IRequestHandler<DeleteFeedbackTypeCommand, ServiceResponse<Guid>>
{
    private readonly IFeedbackService _feedbackService;
    private readonly ILogger<DeleteFeedbackTypeCommandHandler> _logger;

    public DeleteFeedbackTypeCommandHandler(ILogger<DeleteFeedbackTypeCommandHandler> logger, IFeedbackService feedbackService)
    {
        _logger = logger;
        _feedbackService = feedbackService;
    }

    public async Task<ServiceResponse<Guid>> Handle(DeleteFeedbackTypeCommand request, CancellationToken cancellationToken)
    {
        var id = await _feedbackService.DeleteAsync(request.Id, cancellationToken);

        if (id == Guid.Empty)
        {
            _logger.LogWarning("Feedback Type with ID {typeId} not found for deletion.", request.Id);

            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Không tìm thấy loại phản hồi!",
                Data = Guid.Empty
            };
        }

        _logger.LogInformation("Delete updated with ID: {typeId}", id);

        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Xóa loại phản hồi thành công!",
            Data = id
        };
    }
}
