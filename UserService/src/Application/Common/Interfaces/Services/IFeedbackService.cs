using UserService.Application.Feedbacks.Commands.CreateFeedback;
using UserService.Application.Feedbacks.Commands.CreateFeedbackType;
using UserService.Application.Feedbacks.Commands.UpdateFeedbackType;
using UserService.Application.Feedbacks.DTOs;

namespace UserService.Application.Common.Interfaces.Services;

public interface IFeedbackService
{
    Task<Guid> CreateAsync(CreateFeedbackTypeCommand command, CancellationToken cancellationToken = default);
    Task<IEnumerable<FeedbackTypeReadModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreateFeedbackCommand command, string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FeedbackResponseDto>?> GetFeedbacksAsync(string? userId, CancellationToken cancellationToken = default);
    Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> UpdateAsync(UpdateFeedbackTypeCommand command, CancellationToken cancellationToken = default);
    Task<FeedbackTypeReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
