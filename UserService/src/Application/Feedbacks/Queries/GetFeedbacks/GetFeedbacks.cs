using BuildingBlocks.Response;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Feedbacks.DTOs;

namespace UserService.Application.Feedbacks.Queries.GetFeedbacks;

public record GetFeedbacksQuery : IRequest<ServiceResponse<IEnumerable<FeedbackResponseDto>>>;


public class GetFeedbacksQueryHandler : IRequestHandler<GetFeedbacksQuery, ServiceResponse<IEnumerable<FeedbackResponseDto>>>
{
    private readonly IUser _user;
    private readonly IFeedbackService _feedbackService;

    public GetFeedbacksQueryHandler(IUser user, IFeedbackService feedbackService)
    {
        _user = user;
        _feedbackService = feedbackService;
    }

    public async Task<ServiceResponse<IEnumerable<FeedbackResponseDto>>> Handle(GetFeedbacksQuery request, CancellationToken cancellationToken)
    {
        var response = await _feedbackService.GetFeedbacksAsync(_user.Id, cancellationToken);

        return new ServiceResponse<IEnumerable<FeedbackResponseDto>>()
        {
            Succeeded = true,
            Message = "Lấy danh sách feedback thành công!",
            Data = response
        };
    }
}
