namespace UserService.Application.Feedbacks.DTOs;

public class FeedbackResponseDto
{
    public string Type { get; set; } = string.Empty;
    public string? Station { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
