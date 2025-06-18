using UserService.Application.Feedbacks.Commands.CreateFeedbackType;
using UserService.Application.Feedbacks.DTOs;

namespace UserService.Application.Common.Interfaces.Services;

public interface IFeedbackService
{
    Task<Guid> CreateAsync(CreateFeedbackTypeCommand command, CancellationToken cancellationToken = default);
    Task<IEnumerable<FeedbackTypeResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
