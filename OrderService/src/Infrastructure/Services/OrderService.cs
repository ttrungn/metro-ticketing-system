using BuildingBlocks.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderService.Application.Common.Interfaces.Repositories;
using OrderService.Application.Common.Interfaces.Services;
using OrderService.Application.Orders.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientService _httpClientService;

    public OrderService(IUnitOfWork unitOfWork, IHttpClientService httpClientService,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _httpClientService = httpClientService;
        _configuration = configuration;
    }

    public async Task<IEnumerable<TicketDto>> GetUserTicketsAsync(string? userId,
        PurchaseTicketStatus status, CancellationToken cancellationToken = default)
    {
        var baseUrl = Guard.Against.NullOrEmpty(_configuration["ClientSettings:UserServiceClient"],
            message: "User Service Client URL is not configured.");
        var endpoint = $"api/user/Customers";
        var response = await _httpClientService.SendGet<ServiceResponse<CustomerReadModel>>(
            baseUrl,
            endpoint,
            cancellationToken: cancellationToken);
        var customerId = response?.Data?.CustomerId.ToString();

        var repo = _unitOfWork.GetRepository<OrderDetail, Guid>();

        var now = DateTimeOffset.UtcNow;
        var query = repo.Query().Where(od => od.Order.CustomerId == customerId
                                             && od.Order.Status == OrderStatus.Paid);

        if (status == PurchaseTicketStatus.Unused)
        {
            query =
                query.Where(od => od.Status == PurchaseTicketStatus.Unused && od.ExpiredAt > now);
        }
        else if (status == PurchaseTicketStatus.Used)
        {
            query = query.Where(od => od.Status == PurchaseTicketStatus.Used);
        }
        else if (status == PurchaseTicketStatus.Expired)
        {
            query = query.Where(od => od.ExpiredAt < now || od.Status == status);
        }

        var ticketUrl = Guard.Against.NullOrEmpty(_configuration["ClientSettings:CatalogServiceClient"],
            message: "Catalog Service Client URL is not configured.");
        var ticketEndpoint = $"api/catalog/Tickets/filter?page=0&pageSize=100&status=false";
        var ticketResponse = await _httpClientService.SendGet<ServiceResponse<GetTicketsResponseDto>>(
            ticketUrl,
            ticketEndpoint,
            cancellationToken: cancellationToken);

        var stationUrl = Guard.Against.NullOrEmpty(_configuration["ClientSettings:CatalogServiceClient"],
            message: "Catalog Service Client URL is not configured.");
        var stationEndpoint = $"api/catalog/Stations?page=0&pageSize=100&status=false";
        var stationResponse = await _httpClientService.SendGet<ServiceResponse<GetStationsResponseDto>>(
            stationUrl,
            stationEndpoint,
            cancellationToken: cancellationToken);

        var ticketMap = ticketResponse?.Data?.Tickets.ToDictionary(t => t.Id, t => t.Name) ?? new Dictionary<Guid, string?>();
        var stationMap = stationResponse?.Data?.Stations.ToDictionary(s => s.Id, s => s.Name) ?? new Dictionary<Guid, string?>();
        var typeMap = ticketResponse?.Data?.Tickets.ToDictionary(t => t.Id, t => t.TicketType) ?? new Dictionary<Guid, int>();

        var tickets = await query
            .Select(od => new TicketDto
            {
                Id = od.Id,
                OrderId = od.OrderId,
                TicketId = od.TicketId,
                TicketName = ticketMap.GetValueOrDefault(od.TicketId),
                TicketType = typeMap.GetValueOrDefault(od.TicketId),
                BoughtPrice = od.BoughtPrice,
                ActiveAt = od.ActiveAt,
                ExpiredAt = od.ExpiredAt,
                Status = od.Status,
                EntryStationName = stationMap.GetValueOrDefault(Guid.Parse(od.EntryStationId)!),
                DestinationStationName = stationMap.GetValueOrDefault(Guid.Parse(od.DestinationStationId)!)
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return tickets;
    }

    public async Task<(Guid, Guid)> UpdateTicketAsync(
        string? userId,
        Guid id,
        Guid ticketId,
        PurchaseTicketStatus fromStatus,
        PurchaseTicketStatus? toStatus,
        CancellationToken cancellationToken = default)
    {
        var userUrl = Guard.Against.NullOrEmpty(_configuration["ClientSettings:UserServiceClient"],
            message: "User Service Client URL is not configured.");
        var userEndpoint = $"api/user/Customers";
        var userResponse = await _httpClientService.SendGet<ServiceResponse<CustomerReadModel>>(
            userUrl,
            userEndpoint,
            cancellationToken: cancellationToken);
        var customerId = userResponse?.Data?.CustomerId.ToString();

        var ticketUrl = Guard.Against.NullOrEmpty(_configuration["ClientSettings:CatalogServiceClient"],
            message: "Catalog Service Client URL is not configured.");
        var ticketEndpoint = $"api/catalog/Tickets/{ticketId}";
        var ticketResponse = await _httpClientService.SendGet<ServiceResponse<TicketReadModel>>(
            ticketUrl,
            ticketEndpoint,
            cancellationToken: cancellationToken);
        if (ticketResponse.Succeeded == false) return (id, Guid.Empty);
        var ticketModel = ticketResponse?.Data!;

        var repo = _unitOfWork.GetRepository<OrderDetail, Guid>();
        var now = DateTimeOffset.UtcNow;
        var query = repo.Query()
            .Where(od => od.Id == id
                         && od.TicketId == ticketId
                         && od.Order.CustomerId == customerId);
        OrderDetail? ticket = null!;

        if (fromStatus == PurchaseTicketStatus.Unused && toStatus == PurchaseTicketStatus.Used)
        {
            query = query.Where(od => od.Status == PurchaseTicketStatus.Unused && od.ExpiredAt > now);
            ticket = await query
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            if (ticket == null) return (id, Guid.Empty);
            if (ticket.ActiveAt == DateTimeOffset.MinValue)
            {
                ticket.ActiveAt = now;
                ticket.ExpiredAt = now.AddDays(ticketModel.ExpirationInDay);
            }
            ticket.Status = PurchaseTicketStatus.Used;
        }
        else if (fromStatus == PurchaseTicketStatus.Used && ticketModel.TicketType == 1)
        {
            query = query.Where(od => od.Status == PurchaseTicketStatus.Used && od.ExpiredAt > now);
            ticket = await query
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            if (ticket == null) return (id, Guid.Empty);

            ticket.ExpiredAt = now;
            ticket.Status = PurchaseTicketStatus.Expired;
        }
        else if (fromStatus == PurchaseTicketStatus.Used && ticketModel.TicketType != 1)
        {
            query = query.Where(od => od.Status == PurchaseTicketStatus.Used);
            ticket = await query
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            if (ticket == null) return (id, Guid.Empty);
            if (ticket.ExpiredAt < now)
            {
                ticket.Status = PurchaseTicketStatus.Expired;
            }
            else
            {
                ticket.Status = PurchaseTicketStatus.Unused;
            }
        }

        await repo.UpdateAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
        return (id, ticketId);
    }
}
