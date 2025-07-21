using OrderService.Application.MomoPayment.DTOs;
using OrderService.Application.Orders.DTOs;
using OrderService.Domain.Enums;

namespace OrderService.Application.Common.Interfaces.Services;

public interface IOrderService
{
    Task<IEnumerable<TicketDto>> GetUserTicketsAsync(
        string? userId,
        PurchaseTicketStatus status,
        CancellationToken cancellationToken = default);

    Task<(Guid, Guid)> UpdateTicketAsync(
        string? userId,
        Guid id,
        Guid ticketId,
        PurchaseTicketStatus fromStatus,
        PurchaseTicketStatus? toStatus,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateOrderAsync(
        string orderId,
        string? userId,
        List<OrderDetailDto>  orderDetails,
        CancellationToken cancellationToken = default);

    Task<Guid> ConfirmOrder(
        decimal amount,
        string thirdPartyTransactionId, 
        string? userId,
        Guid orderId,
        OrderStatus status,
        string transType,
        CancellationToken cancellationToken = default);
}
