using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Feedbacks.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Feedbacks.Queries.GetUserFeedback;

public record GetUserFeedbackQuery : IRequest<ServiceResponse<GetUserFeedbackResponse>>
{
    public int Page { get; init; } = 0;
    public int PageSize { get; init; } = 8;
    public string? FeedbackTypeId { get; init; } = null!;
    public string? StationId { get; init; } = null!;
    public bool? Status { get; init; } = false;
}

public class GetUserFeedbackQueryValidator : AbstractValidator<GetUserFeedbackQuery>
{
    public GetUserFeedbackQueryValidator()
    {
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Kích thước trang phải lớn hơn 0!");
    }
}

public class GetUserFeedbackQueryHandler : IRequestHandler<GetUserFeedbackQuery, ServiceResponse<GetUserFeedbackResponse>>
{
    private readonly IFeedbackService _feedbackService;
    private readonly ILogger<GetUserFeedbackQueryHandler> _logger;

    public GetUserFeedbackQueryHandler(IFeedbackService feedbackService, ILogger<GetUserFeedbackQueryHandler> logger)
    {
        _feedbackService = feedbackService;
        _logger = logger;
    }

    public async Task<ServiceResponse<GetUserFeedbackResponse>> Handle(GetUserFeedbackQuery request, CancellationToken cancellationToken)
    {
        var (feedbacks, totalPages) = await _feedbackService.GetAsync(request, cancellationToken);

        var response = new GetUserFeedbackResponse
        {
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            Feedbacks = feedbacks,
        };

        _logger.LogInformation("Retrieve feedbacks successfully: Total pages: {TotalPages} - Current page: {CurrentPage} - Page size: {PageSize}",
            response.TotalPages, response.CurrentPage, response.PageSize);
        return new ServiceResponse<GetUserFeedbackResponse>()
        {
            Succeeded = true,
            Message = "Lấy danh sách feedback thành công!",
            Data = response
        };
    }
}
