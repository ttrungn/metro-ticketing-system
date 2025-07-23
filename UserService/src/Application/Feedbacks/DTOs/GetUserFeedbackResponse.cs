namespace UserService.Application.Feedbacks.DTOs;

public class GetUserFeedbackResponse
{
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; } = 0;
    public int PageSize { get; set; } = 8;
    public IEnumerable<UserFeedbackResponse> Feedbacks { get; set; } = new List<UserFeedbackResponse>();
}
