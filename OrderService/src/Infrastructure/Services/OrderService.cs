using BuildingBlocks.Domain.Events.Orders;
using BuildingBlocks.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Repositories;
using OrderService.Application.Common.Interfaces.Services;
using OrderService.Application.MomoPayment.DTOs;
using OrderService.Application.Orders.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientService _httpClientService;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IUnitOfWork unitOfWork, IHttpClientService httpClientService,
        IConfiguration configuration,
        ILogger<OrderService> logger,
        IUser user
        )
    {
        _unitOfWork = unitOfWork;
        _httpClientService = httpClientService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IEnumerable<TicketDto>> GetUserTicketsAsync(string? userId,
        PurchaseTicketStatus status, CancellationToken cancellationToken = default)
    {
        var baseUrl = Guard.Against.NullOrEmpty(_configuration["ClientSettings:UserServiceClient"],
            message: "User Service Client URL is not configured.");
        var endpoint = $"api/user/Customers/profile";
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
                EntryStationName = od.EntryStationId != null ? stationMap.GetValueOrDefault(Guid.Parse(od.EntryStationId)) : null,
                DestinationStationName = od.DestinationStationId != null ? stationMap.GetValueOrDefault(Guid.Parse(od.DestinationStationId)) : null,
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
        var userEndpoint = $"api/user/Customers/profile";
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
            if (ticket.ActiveAt > DateTimeOffset.UtcNow)
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

    public async Task<Guid> CreateOrderAsync(string orderId, string? userId, List<OrderDetailDto> orderDetails, CancellationToken cancellationToken = default)
    {
        var userUrl = Guard.Against.NullOrEmpty(_configuration["ClientSettings:UserServiceClient"],
            message: "User Service Client URL is not configured.");

        var userEndpoint = $"api/user/Customers/profile";

        var userResponse = await _httpClientService.SendGet<ServiceResponse<CustomerReadModel>>(
            userUrl,
            userEndpoint,
            cancellationToken: cancellationToken);

        if (userResponse==null)
        {
           return Guid.Empty;
        }

        var ticketUrl = Guard.Against.NullOrEmpty(_configuration["ClientSettings:CatalogServiceClient"],
            message: "Catalog Service Client URL is not configured.");

        var ticketEndpoint = $"api/catalog/Tickets/filter?page=0&pageSize=100&status=false";

        var ticketResponse = await _httpClientService.SendGet<ServiceResponse<GetTicketsResponseDto>>(
            ticketUrl,
            ticketEndpoint,
            cancellationToken: cancellationToken);

        if (ticketResponse.Succeeded == false)
        {
            return Guid.Empty;
        }

        if (Guid.TryParse(orderId, out var returnedOrderId) == false)
        {
            return Guid.Empty;
        }



        var orderDetailsList = new List<OrderDetail>();
        var tickets = (GetTicketsResponseDto)ticketResponse?.Data!;

        var buyDate = DateTime.Now;
        foreach (var orderDetail in orderDetails)
        {
            var ticket= tickets.Tickets.FirstOrDefault(t => t.Id == orderDetail.TicketId);

            if (ticket == null)
            {
                return Guid.Empty;
            }
            var activeDate = buyDate.AddDays(ticket.ActiveInDay);
            var expiredDate = activeDate.AddDays(ticket.ExpirationInDay);  
            var entryStationId = orderDetail.EntryStationId.HasValue ? orderDetail.EntryStationId.Value.ToString() : null;
            var destinationStationId = orderDetail.DestinationStationId.HasValue ? orderDetail.DestinationStationId.Value.ToString() : null;
            for(var i = 0; i < orderDetail.Quantity; i++)
            {
                var orderDetailEntity = new OrderDetail
                    {
                        Id = new Guid(),
                        OrderId = returnedOrderId,
                        TicketId = orderDetail.TicketId,
                        BoughtPrice = orderDetail.BoughtPrice,
                        ActiveAt = activeDate,
                        ExpiredAt = expiredDate,
                        EntryStationId = entryStationId,
                        DestinationStationId = destinationStationId,
                    };
                    orderDetailsList.Add(orderDetailEntity);
            }

        }
        var order = new Order
        {
            Id = returnedOrderId,
            CustomerId = userResponse?.Data?.CustomerId.ToString()!,
            Status = OrderStatus.Unpaid,
            PaymentMethod = PaymentMethod.MoMo,
            ThirdPartyPaymentId = "",
            OrderDetails = orderDetailsList,
        };

        await _unitOfWork.GetRepository<Order, Guid>().AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
        return returnedOrderId;

    }

    public async Task<int> ConfirmOrder(decimal amount,string thirdPaymentId, IUser user, Guid orderId, OrderStatus status,string transType, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.Query().Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        if (order == null)
        {
            _logger.LogWarning("ConfirmOrder: Cannot found order ID = {OrderId}", orderId);

            return 0;
        }
        order.ThirdPartyPaymentId = thirdPaymentId;
        order.Status = status;
        _logger.LogInformation("ConfirmOrder: Order: {@Order}, Amount: {Amount}, TransactionType: {TransType}", order, amount, transType);
        await repo.UpdateAsync(order, cancellationToken);
        var createOrderEvent = new CreateOrderEvent()
        {
            Email = user.Email!,
            OrderId = order.Id,
            Amount = amount,
            OrderDetails = MapToCreateOrderEventOrderDetails(order.OrderDetails)
        };
        _logger.LogInformation("CreateOrderEvent JSON: {Json}", JsonConvert.SerializeObject(createOrderEvent, Formatting.Indented));
        order.AddDomainEvent(createOrderEvent);
        var transactionHistoryRepo = _unitOfWork.GetRepository<TransactionHistory, Guid>();

        var transactionHistory = new TransactionHistory
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            CustomerId = order.CustomerId,
            Amount = amount,
            TransactionDate = DateTime.UtcNow,
            PaymentMethod = order.PaymentMethod,
            TransactionType = transType,

        };

        await transactionHistoryRepo.AddAsync(transactionHistory);
        _logger.LogInformation("ConfirmOrder: Transaction history added for OrderId: {TransactionHistory}", transactionHistory);
        await _unitOfWork.SaveChangesAsync();

        return order.OrderDetails.Count(t => t.DeleteFlag == false);
    }
    
    private List<CreateOrderEventOrderDetail> MapToCreateOrderEventOrderDetails(List<OrderDetail> orderDetails)
    {
        var grouped = orderDetails
            .GroupBy(od => new
            {
                od.TicketId,
                od.EntryStationId,
                od.DestinationStationId
            })
            .Select(g => new CreateOrderEventOrderDetail
            {
                TicketId = g.Key.TicketId,
                EntryStationId = string.IsNullOrWhiteSpace(g.Key.EntryStationId)
                    ? Guid.Empty
                    : Guid.Parse(g.Key.EntryStationId),
                DestinationStationId = string.IsNullOrWhiteSpace(g.Key.DestinationStationId)
                    ? Guid.Empty
                    : Guid.Parse(g.Key.DestinationStationId),
                Quantity = g.Count(),
                Price = g.Sum(od => od.BoughtPrice)
            })
            .ToList();

        return grouped;
    }
}
