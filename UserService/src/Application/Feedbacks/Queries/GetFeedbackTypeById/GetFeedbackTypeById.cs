using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Feedbacks.DTOs;

namespace UserService.Application.Feedbacks.Queries.GetFeedbackTypeById;

public record GetFeedbackTypeByIdQuery(Guid Id) : IRequest<ServiceResponse<FeedbackTypeResponseDto>>;

public class GetFeedbackTypeByIdQueryValidator : AbstractValidator<GetFeedbackTypeByIdQuery>
{
    public GetFeedbackTypeByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Vui lòng nhập ID của loại phản hồi!");
    }
}

public class GetFeedbackTypeByIdQueryHandler : IRequestHandler<GetFeedbackTypeByIdQuery, ServiceResponse<FeedbackTypeResponseDto>>
{
    private readonly IFeedbackService _feedbackService;
    private readonly ILogger<GetFeedbackTypeByIdQueryHandler> _logger;

    public GetFeedbackTypeByIdQueryHandler(IFeedbackService feedbackService, ILogger<GetFeedbackTypeByIdQueryHandler> logger)
    {
        _feedbackService = feedbackService;
        _logger = logger;
    }

    public async Task<ServiceResponse<FeedbackTypeResponseDto>> Handle(GetFeedbackTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var type = await _feedbackService.GetByIdAsync(request.Id, cancellationToken);

        if (type == null)
        {
            _logger.LogWarning("Feedback type with ID {typeId} not found.", request.Id);
            return new ServiceResponse<FeedbackTypeResponseDto>
            {
                Succeeded = false,
                Message = "Không tìm thấy loại phản hồi!",
                Data = null
            };
        }

        _logger.LogInformation("Feedback type with ID {typeId} retrieved successfully.", request.Id);

        return new ServiceResponse<FeedbackTypeResponseDto>
        {
            Succeeded = true,
            Message = "Lấy thông tin loại phản hồi thành công!",
            Data = type
        };
    }
}
