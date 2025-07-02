using BuildingBlocks.Domain.Common;

namespace UserService.Application.Feedbacks.DTOs;

public class FeedbackTypeReadModel : BaseReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}
