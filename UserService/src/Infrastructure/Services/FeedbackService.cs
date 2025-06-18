using Microsoft.EntityFrameworkCore;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Feedbacks.Commands.CreateFeedbackType;
using UserService.Application.Feedbacks.DTOs;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IUnitOfWork _unitOfWork;

    public FeedbackService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> CreateAsync(CreateFeedbackTypeCommand command, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<FeedbackType, Guid>();

        var id = Guid.NewGuid();
        var newFeedbackType = new FeedbackType()
        {
            Id = id,
            Name = command.Name,
            Description = command.Description
        };

        await repo.AddAsync(newFeedbackType, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return id;
    }

    public async Task<IEnumerable<FeedbackTypeResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<FeedbackType, Guid>();

        return await repo.Query()
            .Select(ft => new FeedbackTypeResponseDto
            {
                Id = ft.Id,
                Name = ft.Name,
                Description = ft.Description
            })
            .ToListAsync(cancellationToken);
    }
}
