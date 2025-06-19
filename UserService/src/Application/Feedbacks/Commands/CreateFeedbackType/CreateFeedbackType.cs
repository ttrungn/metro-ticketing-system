using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Application.Feedbacks.Commands.CreateFeedbackType;

public record CreateFeedbackTypeCommand : IRequest<ServiceResponse<Guid>>
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
}

public class CreateFeedbackTypeCommandValidator : AbstractValidator<CreateFeedbackTypeCommand>
{
    public CreateFeedbackTypeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Vui lòng nhập tên!");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Vui lòng nhâp mô tả!");
    }
}

public class CreateFeedbackTypeCommandHandler : IRequestHandler<CreateFeedbackTypeCommand, ServiceResponse<Guid>>
{
    private readonly IFeedbackService _feedbackService;
    private readonly ILogger<CreateFeedbackTypeCommandHandler> _logger;

    public CreateFeedbackTypeCommandHandler(IFeedbackService feedbackService, ILogger<CreateFeedbackTypeCommandHandler> logger)
    {
        _feedbackService = feedbackService;
        _logger = logger;
    }

    public async Task<ServiceResponse<Guid>> Handle(CreateFeedbackTypeCommand request, CancellationToken cancellationToken)
    {
        var typeId = await _feedbackService.CreateAsync(request, cancellationToken);

        _logger.LogInformation("Feedback type created with ID: {typeId}", typeId);
        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Tạo loại feedback thành công!",
            Data = typeId
        };
    }
}
