using BuildingBlocks.Domain.Common;
using UserService.Application.Users.DTOs;

namespace UserService.Application.Feedbacks.DTOs;

public class FeedbackReadModel : BaseReadModel
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid FeedbackTypeId { get; set; }
    public Guid StationId { get; set; }
    public string Content { get; set; } = null!;
}
