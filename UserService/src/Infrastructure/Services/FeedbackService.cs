using BuildingBlocks.Response;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Feedbacks.Commands.CreateFeedback;
using UserService.Application.Feedbacks.Commands.CreateFeedbackType;
using UserService.Application.Feedbacks.Commands.UpdateFeedbackType;
using UserService.Application.Feedbacks.DTOs;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientService _httpClientService;

    public FeedbackService(IUnitOfWork unitOfWork, IHttpClientService httpClientService)
    {
        _unitOfWork = unitOfWork;
        _httpClientService = httpClientService;
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

    public async Task<Guid> CreateAsync(
        CreateFeedbackCommand command,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
        var customer = await customerRepo.Query().FirstOrDefaultAsync(c => c.ApplicationUserId == userId, cancellationToken: cancellationToken);
        if (customer == null)
        {
            return Guid.Empty;
        }

        var typeRepo = _unitOfWork.GetRepository<FeedbackType, Guid>();
        var type = await typeRepo.Query().FirstOrDefaultAsync(ft => ft.Id == command.FeedbackTypeId, cancellationToken: cancellationToken);
        if (type == null)
        {
            return Guid.Empty;
        }

        var id = Guid.NewGuid();
        var newFeedback = new Feedback()
        {
            Id = id,
            CustomerId = customer.Id,
            FeedbackTypeId = type.Id,
            StationId = Guid.Empty,
            Content = command.Content,
        };

        var endpoint = $"api/catalog/Stations/{Guid.Parse(command.StationId!)}";
        var response = await _httpClientService.SendRequest<ServiceResponse<StationsResponseDto>>(
            endpoint,
            HttpMethod.Get,
            cancellationToken: cancellationToken);

        if (response.Data != null)
        {
            newFeedback.StationId = (Guid)response?.Data?.Id!;
        }

        var feedbackRepo = _unitOfWork.GetRepository<Feedback, Guid>();
        await feedbackRepo.AddAsync(newFeedback, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return id;
    }

    public async Task<IEnumerable<FeedbackResponseDto>?> GetFeedbacksAsync(string? userId, CancellationToken cancellationToken = default)
    {
        var endpoint = $"api/catalog/Stations?page=0&pageSize=100&status=false";
        var response = await _httpClientService.SendRequest<ServiceResponse<GetStationsResponseDto>>(
            endpoint,
            HttpMethod.Get,
            cancellationToken: cancellationToken);
        var map = response?.Data?.Stations.ToDictionary(s => s.Id, s => s.Name) ?? new Dictionary<Guid, string?>();

        var feedbackRepo = _unitOfWork.GetRepository<Feedback, Guid>();
        var feedbacks = await feedbackRepo.Query()
            .Where(f => f.Customer.ApplicationUserId == userId).Include(f => f.FeedbackType)
            .ToListAsync(cancellationToken);

        var result = feedbacks.Select(f => new FeedbackResponseDto
        {
            Type = f.FeedbackType.Name,
            Station = (f.StationId != Guid.Empty && map.TryGetValue(f.StationId, out var name)) ? name : "",
            Content = f.Content
        }).ToList();

        return result;
    }

    public async Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<FeedbackType, Guid>();
        var type = await repo.Query().FirstOrDefaultAsync(ft => ft.Id == id, cancellationToken: cancellationToken);

        if (type == null)
        {
            return Guid.Empty;
        }

        type.DeleteFlag = true;

        await repo.UpdateAsync(type, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return type.Id;
    }

    public async Task<Guid> UpdateAsync(UpdateFeedbackTypeCommand command, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<FeedbackType, Guid>();
        var type = await repo.Query().FirstOrDefaultAsync(ft => ft.Id == command.Id, cancellationToken: cancellationToken);

        if (type == null)
        {
            return Guid.Empty;
        }

        type.Name = command.Name!;
        type.Description = command.Description!;

        await repo.UpdateAsync(type, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return type.Id;
    }

    public Task<FeedbackTypeResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<FeedbackType, Guid>();
        return repo.Query()
            .Where(ft => ft.Id == id)
            .Select(ft => new FeedbackTypeResponseDto
            {
                Id = ft.Id,
                Name = ft.Name,
                Description = ft.Description
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
