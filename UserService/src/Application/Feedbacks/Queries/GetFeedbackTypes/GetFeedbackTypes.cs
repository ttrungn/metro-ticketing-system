using BuildingBlocks.Response;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Feedbacks.DTOs;

namespace UserService.Application.Feedbacks.Queries.GetFeedbackTypes;

public record GetFeedbackTypesQuery : IRequest<ServiceResponse<IEnumerable<FeedbackTypeReadModel>>>;

public class GetFeedbackTypesQueryHandler : IRequestHandler<GetFeedbackTypesQuery, ServiceResponse<IEnumerable<FeedbackTypeReadModel>>>
{
    private readonly IFeedbackService _feedbackService;

    public GetFeedbackTypesQueryHandler(IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    public async Task<ServiceResponse<IEnumerable<FeedbackTypeReadModel>>> Handle(GetFeedbackTypesQuery request, CancellationToken cancellationToken)
    {
        var feedbackTypes = await _feedbackService.GetAllAsync(cancellationToken);

        return new ServiceResponse<IEnumerable<FeedbackTypeReadModel>>()
        {
            Succeeded = true,
            Message = "Lấy danh sách loại feedback thành công!",
            Data = feedbackTypes
        };
    }
}
