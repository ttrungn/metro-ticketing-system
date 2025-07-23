using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.Application.Feedbacks.DTOs;

public class UserFeedbackResponse
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public FullName FullName { get; set; } = null!;
    public Guid StationId { get; set; }
    public string? StationName { get; set; } = null!;
    public string Content { get; set; } = null!;
    public FeedbackType FeedbackType { get; set; } = null!;
}
