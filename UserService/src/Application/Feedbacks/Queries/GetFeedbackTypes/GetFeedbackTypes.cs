using BuildingBlocks.Response;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Feedbacks.DTOs;

namespace UserService.Application.Feedbacks.Queries.GetFeedbackTypes;

public record GetFeedbackTypesQuery : IRequest<ServiceResponse<IEnumerable<FeedbackTypeResponseDto>>>;

public class GetFeedbackTypesQueryHandler : IRequestHandler<GetFeedbackTypesQuery, ServiceResponse<IEnumerable<FeedbackTypeResponseDto>>>
{
    private readonly IFeedbackService _feedbackService;

    public GetFeedbackTypesQueryHandler(IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    public async Task<ServiceResponse<IEnumerable<FeedbackTypeResponseDto>>> Handle(GetFeedbackTypesQuery request, CancellationToken cancellationToken)
    {
        var feedbackTypes = await _feedbackService.GetAllAsync(cancellationToken);

        return new ServiceResponse<IEnumerable<FeedbackTypeResponseDto>>()
        {
            Succeeded = true,
            Message = "Lấy danh sách loại feedback thành công!",
            Data = feedbackTypes
        };
    }
}
