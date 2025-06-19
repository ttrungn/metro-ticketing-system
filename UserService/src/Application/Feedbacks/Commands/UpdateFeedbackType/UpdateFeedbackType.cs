using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Application.Feedbacks.Commands.UpdateFeedbackType;

public record UpdateFeedbackTypeCommand : IRequest<ServiceResponse<Guid>>
{
    public Guid Id { get; init; } = Guid.Empty;
    public string? Name { get; init; }
    public string? Description { get; init; }
}

public class UpdateFeedbackTypeCommandValidator : AbstractValidator<UpdateFeedbackTypeCommand>
{
    public UpdateFeedbackTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Xin vui lòng nhập ID của loại phản hồi!");
    }
}

public class UpdateFeedbackTypeCommandHandler : IRequestHandler<UpdateFeedbackTypeCommand, ServiceResponse<Guid>>
{
    private readonly IFeedbackService _feedbackService;
    private readonly ILogger<UpdateFeedbackTypeCommandHandler> _logger;

    public UpdateFeedbackTypeCommandHandler(IFeedbackService feedbackService, ILogger<UpdateFeedbackTypeCommandHandler> logger)
    {
        _feedbackService = feedbackService;
        _logger = logger;
    }

    public async Task<ServiceResponse<Guid>> Handle(UpdateFeedbackTypeCommand request, CancellationToken cancellationToken)
    {
        var id = await _feedbackService.UpdateAsync(request, cancellationToken);
        if (id == Guid.Empty)
        {
            _logger.LogWarning("Feedback type with ID {typeId} not found.", request.Id);
            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Không tìm thấy loại phản hồi!",
                Data = Guid.Empty
            };
        }

        _logger.LogInformation("Feedback type updated with ID: {RouteId}", id);
        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Chỉnh sửa loại phản hồi thành công!",
            Data = id
        };
    }
}
