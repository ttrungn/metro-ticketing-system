using System.Net.Http.Json;
using BuildingBlocks.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Feedbacks.Commands.CreateFeedback;
using UserService.Application.Feedbacks.Commands.CreateFeedbackType;
using UserService.Application.Feedbacks.DTOs;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiBaseUrl;

    public FeedbackService(IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
        _apiBaseUrl = configuration["ClientSettings:ApiBaseUrl"] ?? "http://localhost:8080";
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

        var cilent = _httpClientFactory.CreateClient();
        var response = await cilent.GetAsync($"{_apiBaseUrl}/api/catalog/Stations/{Guid.Parse(command.StationId!)}", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadFromJsonAsync<ServiceResponse<StationsResponseDto>>(cancellationToken: cancellationToken);
            newFeedback.StationId = (Guid)responseData?.Data?.Id!;
        }

        var feedbackRepo = _unitOfWork.GetRepository<Feedback, Guid>();
        await feedbackRepo.AddAsync(newFeedback, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return id;
    }

    public async Task<IEnumerable<FeedbackResponseDto>?> GetFeedbacksAsync(string? userId, CancellationToken cancellationToken = default)
    {
        var feedbackRepo = _unitOfWork.GetRepository<Feedback, Guid>();

        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"{_apiBaseUrl}/api/catalog/Stations?page=0&pageSize=100&status=false", cancellationToken);
        var stations = await response.Content.ReadFromJsonAsync<ServiceResponse<GetStationsResponseDto>>(cancellationToken: cancellationToken);
        var stationMap = stations?.Data?.Stations.ToDictionary(s => s.Id, s => s.Name) ?? new Dictionary<Guid, string?>();

        var feedbacks = await feedbackRepo.Query()
            .Where(f => f.Customer.ApplicationUserId == userId).Include(f => f.FeedbackType)
            .ToListAsync(cancellationToken);

        var result = feedbacks.Select(f => new FeedbackResponseDto
        {
            Type = f.FeedbackType.Name,
            Station = (f.StationId != Guid.Empty && stationMap.TryGetValue(f.StationId, out var name)) ? name : "",
            Content = f.Content
        }).ToList();

        return result;
    }
}
