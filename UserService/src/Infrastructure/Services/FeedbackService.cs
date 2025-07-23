using System.Linq.Expressions;
using BuildingBlocks.Domain.Events.Feedbacks;
using BuildingBlocks.Domain.Events.FeedbackTypes;
using BuildingBlocks.Response;
using Marten;
using Marten.Linq.Includes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Feedbacks.Commands.CreateFeedback;
using UserService.Application.Feedbacks.Commands.CreateFeedbackType;
using UserService.Application.Feedbacks.Commands.UpdateFeedbackType;
using UserService.Application.Feedbacks.DTOs;
using UserService.Application.Feedbacks.Queries.GetUserFeedback;
using UserService.Application.Users.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;
using UserService.Infrastructure.Services.Identity;

namespace UserService.Infrastructure.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientService _httpClientService;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;

    public FeedbackService(IUnitOfWork unitOfWork, IHttpClientService httpClientService,
        IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _httpClientService = httpClientService;
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<Guid> CreateAsync(CreateFeedbackTypeCommand command,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<FeedbackType, Guid>();

        var id = Guid.NewGuid();
        var newFeedbackType = new FeedbackType()
        {
            Id = id, Name = command.Name, Description = command.Description
        };

        newFeedbackType.AddDomainEvent(new CreateFeedbackTypeEvent()
        {
            Id = id, Name = command.Name, Description = command.Description
        });

        await repo.AddAsync(newFeedbackType, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return id;
    }

    public async Task<IEnumerable<FeedbackTypeReadModel>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var session = _unitOfWork.GetDocumentSession();
        var feedbackTypes = await QueryableExtensions.ToListAsync(session
                .Query<FeedbackTypeReadModel>()
                .Where(ft => ft.DeleteFlag == false)
                .AsNoTracking(),
            cancellationToken);
        return feedbackTypes;
    }

    public async Task<Guid> CreateAsync(
        CreateFeedbackCommand command,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
        var customer = await customerRepo.Query()
            .FirstOrDefaultAsync(c => c.ApplicationUserId == userId,
                cancellationToken: cancellationToken);
        if (customer == null)
        {
            return Guid.Empty;
        }

        var typeRepo = _unitOfWork.GetRepository<FeedbackType, Guid>();
        var type = await typeRepo.Query().FirstOrDefaultAsync(ft => ft.Id == command.FeedbackTypeId,
            cancellationToken: cancellationToken);
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

        var baseUrl = Guard.Against.NullOrEmpty(
            _configuration["ClientSettings:CatalogServiceClient"],
            message: "Catalog Service Client URL is not configured.");
        var endpoint = $"api/catalog/Stations/{Guid.Parse(command.StationId!)}";
        var response = await _httpClientService.SendGet<ServiceResponse<StationReadModel>>(
            baseUrl,
            endpoint,
            cancellationToken: cancellationToken);

        if (response.Data != null)
        {
            newFeedback.StationId = (Guid)response?.Data?.Id!;
        }

        newFeedback.AddDomainEvent(new CreateFeedbackEvent()
        {
            Id = newFeedback.Id,
            CustomerId = newFeedback.CustomerId,
            FeedbackTypeId = newFeedback.FeedbackTypeId,
            StationId = newFeedback.StationId,
            Content = newFeedback.Content
        });

        var feedbackRepo = _unitOfWork.GetRepository<Feedback, Guid>();
        await feedbackRepo.AddAsync(newFeedback, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return id;
    }

    public async Task<IEnumerable<FeedbackResponseDto>?> GetFeedbacksAsync(string? userId,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = Guard.Against.NullOrEmpty(
            _configuration["ClientSettings:CatalogServiceClient"],
            message: "Catalog Service Client URL is not configured.");
        var endpoint = $"api/catalog/Stations?page=0&pageSize=100&status=false";
        var response = await _httpClientService.SendGet<ServiceResponse<GetStationsResponseDto>>(
            baseUrl,
            endpoint,
            cancellationToken: cancellationToken);

        var session = _unitOfWork.GetDocumentSession();

        var customer = await QueryableExtensions.FirstOrDefaultAsync(session
                .Query<CustomerReadModel>()
                .Where(c => c.Id == userId)
                .AsNoTracking(),
            cancellationToken);

        var typeMap = new Dictionary<Guid, FeedbackTypeReadModel>();
        var stationMap = response?.Data?.Stations.ToDictionary(s => s.Id, s => s.Name) ??
                         new Dictionary<Guid, string>();

        var feedbacks = await QueryableExtensions.ToListAsync(session.Query<FeedbackReadModel>()
                .Include(typeMap).On(f => f.FeedbackTypeId)
                .Where(f => f.CustomerId == customer!.CustomerId),
            cancellationToken);

        var result = feedbacks.Select(f =>
        {
            var type = typeMap.TryGetValue(f.FeedbackTypeId, out var t) ? t?.Name : "Unknown";
            var station = stationMap.TryGetValue(f.StationId, out var s)! ? s : "Unknown";

            return new FeedbackResponseDto
            {
                Type = type ?? "Unknown", Station = station, Content = f.Content
            };
        }).ToList();

        return result;
    }

    public async Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<FeedbackType, Guid>();
        var type = await repo.Query()
            .FirstOrDefaultAsync(ft => ft.Id == id, cancellationToken: cancellationToken);

        if (type == null)
        {
            return Guid.Empty;
        }

        type.DeleteFlag = true;

        type.AddDomainEvent(new DeleteFeedbackTypeEvent() { Id = type.Id, });

        await repo.UpdateAsync(type, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return type.Id;
    }

    public async Task<Guid> UpdateAsync(UpdateFeedbackTypeCommand command,
        CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<FeedbackType, Guid>();
        var type = await repo.Query().FirstOrDefaultAsync(ft => ft.Id == command.Id,
            cancellationToken: cancellationToken);

        if (type == null)
        {
            return Guid.Empty;
        }

        type.Name = command.Name!;
        type.Description = command.Description!;

        type.AddDomainEvent(new UpdateFeedbackTypeEvent
        {
            Id = type.Id, Name = type.Name, Description = type.Description!
        });

        await repo.UpdateAsync(type, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return type.Id;
    }

    public async Task<FeedbackTypeReadModel?> GetByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var session = _unitOfWork.GetDocumentSession();
        var feedbackType = await QueryableExtensions.FirstOrDefaultAsync(session
                .Query<FeedbackTypeReadModel>()
                .Where(ft => ft.Id == id)
                .AsNoTracking(),
            cancellationToken);
        return feedbackType;
    }

    public async Task<(IEnumerable<UserFeedbackResponse>, int)> GetAsync(
        GetUserFeedbackQuery request, CancellationToken cancellationToken = default)
    {
        var feedbackRepo = _unitOfWork.GetRepository<Feedback, Guid>();
        var query = feedbackRepo.Query()
            .Include(f => f.Customer)
            .Include(f => f.FeedbackType)
            .Where(f => (request.FeedbackTypeId == null || f.FeedbackTypeId == Guid.Parse(request.FeedbackTypeId!)) &&
                        (request.StationId == null || f.StationId == Guid.Parse(request.StationId!)) &&
                        f.DeleteFlag == request.Status);

        var users = await _userManager.GetUsersInRoleAsync("Customer");
        var userMap = users.ToDictionary(u => u.Id, u => u.FullName);

        var baseUrl = Guard.Against.NullOrEmpty(
            _configuration["ClientSettings:CatalogServiceClient"],
            message: "Catalog Service Client URL is not configured.");
        var endpoint = $"api/catalog/Stations?page=0&pageSize=100&status=false";
        var response = await _httpClientService.SendGet<ServiceResponse<GetStationsResponseDto>>(
            baseUrl,
            endpoint,
            cancellationToken: cancellationToken);
        var stationMap = response?.Data?.Stations.ToDictionary(s => s.Id, s => s.Name) ??
                         new Dictionary<Guid, string>();

        var feedbacks = await EntityFrameworkQueryableExtensions.ToListAsync(query
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new UserFeedbackResponse
            {
                Id = f.Id,
                CustomerId = f.CustomerId,
                FullName =
                    userMap.GetValueOrDefault(f.Customer.ApplicationUserId!,
                        new FullName("Unknown", "User")),
                StationId = f.StationId,
                StationName = stationMap.GetValueOrDefault(f.StationId, "Unknown"),
                Content = f.Content,
                FeedbackType = f.FeedbackType
            }), cancellationToken);

        var totalCount =
            await EntityFrameworkQueryableExtensions.CountAsync(query.AsNoTracking(),
                cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return (feedbacks, totalPages);
    }
}
