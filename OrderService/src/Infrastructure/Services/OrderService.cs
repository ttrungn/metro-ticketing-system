
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

    public OrderService(IUnitOfWork unitOfWork, IHttpClientService httpClientService, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _httpClientService = httpClientService;
        _configuration = configuration;
    }

    public async Task<IEnumerable<TicketDto>> GetUserTicketsAsync(string? userId, PurchaseTicketStatus status, CancellationToken cancellationToken = default)
    {
        var baseUrl = Guard.Against.NullOrEmpty(_configuration["ClientSettings:UserServiceClient"], message: "User Service Client URL is not configured.");
        var endpoint = $"api/user/Customers";
        var response = await _httpClientService.SendGet<ServiceResponse<CustomerReadModel>>(
            baseUrl,
            endpoint,
            cancellationToken: cancellationToken);
        var customerId = response?.Data?.CustomerId.ToString();

        var repo = _unitOfWork.GetRepository<OrderDetail, Guid>();
        var tickets = await repo
            .Query()
            .Where(od => od.Order.CustomerId == customerId
                         && od.Order.Status == OrderStatus.Paid
                         && od.Status == status)
            .Select(od => new TicketDto
            {
                OrderId = od.OrderId,
                TicketId = od.TicketId,
                BoughtPrice = od.BoughtPrice,
                ActiveAt = od.ActiveAt,
                ExpiredAt = od.ExpiredAt,
                Status = od.Status,
                EntryStationId = od.EntryStationId,
                DestinationStationId = od.DestinationStationId
            })
            .ToListAsync(cancellationToken);

        return tickets;
    }
}
