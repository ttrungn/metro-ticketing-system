namespace UserService.Application.Feedbacks.DTOs;

public class FeedbackTypeResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;
}
